using System.Security.Cryptography;

namespace IonLib.cryptoservices
{
	public static class PBKDF2
	{
		const int Iterations = 1000;

		public static byte[] Encrypt(string password, byte[] salt)
		{
			Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations);
			return pbkdf2.GetBytes(16);
		}

		public static byte[] GenerateSalt()
		{
			byte[] salt = new byte[8];

			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			rng.GetBytes(salt);

			return salt;
		}
	}
}