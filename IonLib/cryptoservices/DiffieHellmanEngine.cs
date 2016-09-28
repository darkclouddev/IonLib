using System;
using System.Text;
using IonLib.cryptoservices.util;

namespace IonLib.cryptoservices
{
	public class DiffieHellmanEngine : IDisposable
	{
		static StrongNumberProvider _strongRng = new StrongNumberProvider();

		//The number of bits to generate.
		readonly int bits;

		//The shared prime.
		BigInteger prime;

		//The shared base.
		BigInteger sharedBase;

		//The private prime.
		BigInteger privatePrime;

		//The string representation/packet.
		string representation;

		//Gets the final key to use for encryption.
		public byte[] Key { get; private set; }

		public DiffieHellmanEngine(int bits)
		{
			this.bits = bits;
		}

		~DiffieHellmanEngine()
		{
			Dispose();
		}

		//Generates a request packet.
		public DiffieHellmanEngine GenerateRequest()
		{
			// Generate the parameters.
			prime = BigInteger.GenPseudoPrime(bits, 30, _strongRng);
			privatePrime = BigInteger.GenPseudoPrime(bits, 30, _strongRng);
			sharedBase = 5;

			// Gemerate the string.
			StringBuilder rep = new StringBuilder();
			rep.Append(prime.ToString(36));
			rep.Append("|");
			rep.Append(sharedBase.ToString(36));
			rep.Append("|");

			// Generate the send BigInt.
			using (BigInteger send = sharedBase.ModPow(privatePrime, prime))
			{
				rep.Append(send.ToString(36));
			}

			representation = rep.ToString();
			return this;
		}

		//Generate a response packet
		public DiffieHellmanEngine GenerateResponse(string request)
		{
			string[] parts = request.Split('|');

			// Generate the would-be fields.
			using (BigInteger prime = new BigInteger(parts[0], 36))
			{
				using (BigInteger g = new BigInteger(parts[1], 36))
				{
					using (BigInteger mine = BigInteger.GenPseudoPrime(bits, 30, _strongRng))
					{
						// Generate the key.
						using (BigInteger given = new BigInteger(parts[2], 36))
						{
							using (BigInteger key = given.ModPow(mine, prime))
							{
								Key = key.GetBytes();
							}
						}

						// Generate the response.
						using (BigInteger send = g.ModPow(mine, prime))
						{
							representation = send.ToString(36);
						}
					}
				}
			}
			return this;
		}

		//Generates the key after a response is received
		public void HandleResponse(string response)
		{
			// Get the response and modpow it with the stored prime.
			using (BigInteger given = new BigInteger(response, 36))
			{
				using (BigInteger key = given.ModPow(privatePrime, prime))
				{
					Key = key.GetBytes();
				}
			}
			Dispose();
		}

		public override string ToString()
		{
			return representation;
		}

		//Ends the calculation. The key will still be available
		public void Dispose()
		{
			if (!ReferenceEquals(prime, null)) prime.Dispose();
			if (!ReferenceEquals(privatePrime, null)) privatePrime.Dispose();
			if (!ReferenceEquals(sharedBase, null)) sharedBase.Dispose();

			prime = null;
			privatePrime = null;
			sharedBase = null;

			representation = null;
			GC.Collect();
			GC.Collect();
		}
	}
}