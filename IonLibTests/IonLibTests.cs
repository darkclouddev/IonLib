using System;
using System.Text;
using IonLib.cryptoservices;
using IonLib.network;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IonLibTests
{
	/// <summary>
	/// Tests class for IonLib
	/// </summary>
	[TestClass]
	public class IonLibTests
	{
		[TestMethod]
		public void DiffieHellmannGeneration()
		{
			DiffieHellmanEngine server = new DiffieHellmanEngine(512).GenerateRequest();
			DiffieHellmanEngine client = new DiffieHellmanEngine(512).GenerateResponse(server.ToString());
			server.HandleResponse(client.ToString());

			Console.Write($"server key={BitConverter.ToString(server.Key)}");
			Console.Write($"client key={BitConverter.ToString(client.Key)}");

			Assert.AreEqual(Convert.ToBase64String(server.Key), Convert.ToBase64String(client.Key));
		}

		[TestMethod]
		public void AESEncryption()
		{
			byte[] key = Encoding.UTF8.GetBytes("testaeskeyconstant");
			string iv = Generator.NextIVForAES();
			byte[] message = Encoding.UTF8.GetBytes("This message will be encrypted here and decrypted by other side");

			byte[] result = AES256.Decrypt(AES256.Encrypt(message, key, iv), key, iv);

			Assert.AreEqual(Encoding.UTF8.GetString(message), Encoding.UTF8.GetString(result));
		}

		[TestMethod]
		public void InsertBytesAtIndex()
		{
			byte[] source = { 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F };
			byte[] insert = { 0xFA, 0xA4 };
			byte[] validResult = { 0x70, 0x71, 0xFA, 0xA4, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F };
			const int index = 2;

			byte[] result = Protocol.InsertBytesAtIndex(index, insert, source);

			Assert.IsTrue(source.Length < result.Length && (result.Length - source.Length) == index);

			for (int i = 0; i < validResult.Length; i++)
			{
				Console.WriteLine($"Assert.AreEqual({result[i]}, {validResult[i]});");
				Assert.AreEqual(result[i], validResult[i]);
			}
		}

		[TestMethod]
		public void ReadBytesAtIndex()
		{
			byte[] source = { 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F };
			byte[] validResult = { 0x74, 0x75, 0x76 };
			const int index = 4;
			const int length = 3;

			byte[] result = Protocol.ReadBytesAtIndex(index, length, source);

			Assert.AreEqual(validResult.Length, result.Length);

			Console.WriteLine($"validResult: {BitConverter.ToString(validResult).Replace("-", "")}");
			Console.WriteLine($"result: {BitConverter.ToString(result).Replace("-", "")}");

			for (int i = 0; i < length; i++)
			{
				Console.WriteLine($"AreEqual({result[i]}, {validResult[i]});");
				Assert.AreEqual(result[i], validResult[i]);
			}
		}

		[TestMethod]
		public void GeneratorIV()
		{
			string num1 = Generator.NextIVForAES();
			string num2 = Generator.NextIVForAES();

			while (num2 == num1) //there is a small chance of collision due to small size of IV
			{
				num2 = Generator.NextIVForAES();
			}

			Console.WriteLine($"{num1} | {num2}");
			Assert.IsTrue(num2 != num1);
		}
	}
}