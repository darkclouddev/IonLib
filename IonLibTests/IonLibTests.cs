using System;
using System.Text;

using IonLib.Cryptoservices;
using IonLib.Network;

using Xunit;

namespace IonLibTests
{
	public class IonLibTests
	{
		[Fact]
		public void DiffieHellmannGeneration()
		{
			DiffieHellmanEngine server = new DiffieHellmanEngine(512).GenerateRequest();
			DiffieHellmanEngine client = new DiffieHellmanEngine(512).GenerateResponse(server.ToString());
			server.HandleResponse(client.ToString());

			//Console.Write($"server key={BitConverter.ToString(server.Key)}");
			//Console.Write($"client key={BitConverter.ToString(client.Key)}");

			Assert.Equal(Convert.ToBase64String(server.Key), Convert.ToBase64String(client.Key));
		}

		[Fact]
		public void AESEncryption()
		{
			byte[] key = Encoding.UTF8.GetBytes("testaeskeyconstant");
			string iv = Generator.NextIVForAES();
			byte[] message = Encoding.UTF8.GetBytes("This message will be encrypted here and decrypted by other side");

			byte[] result = new Aes(256).Decrypt(new Aes(256).Encrypt(message, key, iv), key, iv);

			Assert.Equal(Encoding.UTF8.GetString(message), Encoding.UTF8.GetString(result));
		}

		[Fact]
		public void InsertBytesAtIndex()
		{
			byte[] source = { 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F };
			byte[] insert = { 0xFA, 0xA4 };
			byte[] validResult = { 0x70, 0x71, 0xFA, 0xA4, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F };
			const int index = 2;

			byte[] result = Protocol.InsertBytesAtIndex(index, insert, source);

			Assert.True(source.Length < result.Length && (result.Length - source.Length) == index);

			for (int i = 0; i < validResult.Length; i++)
			{
				//Console.WriteLine($"Assert.AreEqual({result[i]}, {validResult[i]});");
				Assert.Equal(result[i], validResult[i]);
			}
		}

		[Fact]
		public void ReadBytesAtIndex()
		{
			byte[] source = { 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F };
			byte[] validResult = { 0x74, 0x75, 0x76 };
			const int index = 4;
			const int length = 3;

			byte[] result = Protocol.ReadBytesAtIndex(index, length, source);

			Assert.Equal(validResult.Length, result.Length);

			//Console.WriteLine($"validResult: {BitConverter.ToString(validResult).Replace("-", "")}");
			//Console.WriteLine($"result: {BitConverter.ToString(result).Replace("-", "")}");

			for (int i = 0; i < length; i++)
			{
				//Console.WriteLine($"AreEqual({result[i]}, {validResult[i]});");
				Assert.Equal(result[i], validResult[i]);
			}
		}

		[Fact]
		public void GeneratorIV()
		{
			string num1 = Generator.NextIVForAES();

			Assert.True(num1.Length == 16);
		}
	}
}