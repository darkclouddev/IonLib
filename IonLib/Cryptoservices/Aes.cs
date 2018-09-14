using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Text;

namespace IonLib.Cryptoservices
{
	public class Aes
	{
		const int Iterations = 2048;

		readonly int keySize;

		public Aes(int keySize)
		{
			this.keySize = keySize;
		}

		public byte[] Encrypt(byte[] data, byte[] password, byte[] iv)
		{
			return Process(data, password, iv, Mode.Encrypt);
		}

		public byte[] Encrypt(byte[] data, byte[] password, string iv)
		{
			return Encrypt(data, password, Encoding.UTF8.GetBytes(iv));
		}
		
		public byte[] Decrypt(byte[] data, byte[] password, byte[] iv)
		{
			return Process(data, password, iv, Mode.Decrypt);
		}

		public byte[] Decrypt(byte[] data, byte[] password, string iv)
		{
			return Decrypt(data, password, Encoding.UTF8.GetBytes(iv));
		}

		public async Task<byte[]> EncryptAsync(byte[] data, byte[] password, byte[] iv)
		{
			return await ProcessAsync(data, password, iv, Mode.Encrypt);
		}

		public async Task<byte[]> EncryptAsync(byte[] data, byte[] password, string iv)
		{
			return await EncryptAsync(data, password, Encoding.UTF8.GetBytes(iv));
		}

		public async Task<byte[]> DecryptAsync(byte[] data, byte[] password, byte[] iv)
		{
			return await ProcessAsync(data, password, iv, Mode.Decrypt);
		}

		public async Task<byte[]> DecryptAsync(byte[] data, byte[] password, string iv)
		{
			return await DecryptAsync(data, password, Encoding.UTF8.GetBytes(iv));
		}

		byte[] Process(byte[] data, byte[] password, byte[] iv, Mode mode)
		{
			byte[] cryptData;

			using (var aes = CreateAes(password, iv))
			using (var ms = new MemoryStream())
			{
				using (var cs = CreateCryptoStream(ms, aes, mode))
				{
					cs.Write(data, 0, data.Length);
					cs.FlushFinalBlock();
				}

				cryptData = ms.ToArray();
			}

			return cryptData;
		}

		async Task<byte[]> ProcessAsync(byte[] data, byte[] password, byte[] iv, Mode mode)
		{
			byte[] cryptData;

			using (var aes = CreateAes(password, iv))
			using (var ms = new MemoryStream())
			{
				using (var cs = CreateCryptoStream(ms, aes, mode))
				{
					await cs.WriteAsync(data, 0, data.Length)
						.ContinueWith(x =>
						{
							cs.FlushFinalBlock();
						});
				}

				cryptData = ms.ToArray();
			}

			return cryptData;
		}

		AesManaged CreateAes(byte[] password, byte[] iv)
		{
			var key = new Rfc2898DeriveBytes(password, iv, Iterations);
			var aes = new AesManaged();

			aes.KeySize = keySize;
			aes.Key = key.GetBytes(aes.KeySize / 8);
			aes.IV = key.GetBytes(aes.BlockSize / 8);
			aes.Padding = PaddingMode.PKCS7;

			return aes;
		}

		CryptoStream CreateCryptoStream(MemoryStream ms, AesManaged aes, Mode mode)
		{
			return new CryptoStream(
				ms,
				mode == Mode.Encrypt
					? aes.CreateEncryptor()
					: aes.CreateDecryptor(),
				CryptoStreamMode.Write);
		}
	}

	public enum Mode
	{
		Encrypt,
		Decrypt,
	}
}