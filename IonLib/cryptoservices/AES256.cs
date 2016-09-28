using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace IonLib.cryptoservices
{
	public static class AES256
	{
		const int Iterations = 2048;

		public static byte[] Encrypt(byte[] data, byte[] password, byte[] iv)
		{
			byte[] encryptedData;

			Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, iv, Iterations);

			using (var aes = new AesManaged())
			{
				aes.KeySize = 256;
				aes.Key = key.GetBytes(aes.KeySize / 8);
				aes.IV = key.GetBytes(aes.BlockSize / 8);
				aes.Padding = PaddingMode.PKCS7;

				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
					{
						cs.Write(data, 0, data.Length);
						cs.FlushFinalBlock();
					}

					encryptedData = ms.ToArray();
				}
			}

			return encryptedData;
		}

		public static byte[] Decrypt(byte[] data, byte[] password, byte[] iv)
		{
			byte[] decryptedData;

			Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, iv, Iterations);

			using (var aes = new AesManaged())
			{
				aes.KeySize = 256;
				aes.Key = key.GetBytes(aes.KeySize / 8);
				aes.IV = key.GetBytes(aes.BlockSize / 8);
				aes.Padding = PaddingMode.PKCS7;

				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
					{
						cs.Write(data, 0, data.Length);
						cs.FlushFinalBlock();
					}

					decryptedData = ms.ToArray();
				}
			}

			return decryptedData;
		}

		public static byte[] Encrypt(byte[] data, byte[] password, string iv)
		{
			return Encrypt(data, password, Encoding.UTF8.GetBytes(iv));
		}
		public static byte[] Decrypt(byte[] data, byte[] password, string iv)
		{
			return Decrypt(data, password, Encoding.UTF8.GetBytes(iv));
		}
	}
}