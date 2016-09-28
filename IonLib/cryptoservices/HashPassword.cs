﻿using System;
using System.Security.Cryptography;

namespace IonLib.cryptoservices
{
	public static class HashPassword
	{
		public const int SaltByteSize = 24;
		public const int HashByteSize = 24;
		public const int PBKDF2Iterations = 1000;

		public const int IterationIndex = 0;
		public const int SaltIndex = 1;
		public const int PBKDF2Index = 2;

		/// <summary>
		/// Creates a salted PBKDF2 hash of the password.
		/// </summary>
		/// <param name="password">The password to hash.</param>
		/// <returns>The hash of the password.</returns>
		public static string CreateHash(string password)
		{
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			byte[] salt = new byte[SaltByteSize];
			rng.GetBytes(salt);

			byte[] hash = PBKDF2(password, salt, PBKDF2Iterations, HashByteSize);

			return PBKDF2Iterations + ":" + Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
		}

		/// <summary>
		/// Validates a password given a hash of the correct one.
		/// </summary>
		/// <param name="password">The password to check.</param>
		/// <param name="correctHash">A hash of the correct password.</param>
		/// <returns>True if the password is correct. False otherwise.</returns>
		public static bool ValidatePassword(string password, string correctHash)
		{
			string[] split = correctHash.Split(':');
			int iterations = Int32.Parse(split[IterationIndex]);
			byte[] salt = Convert.FromBase64String(split[SaltIndex]);
			byte[] hash = Convert.FromBase64String(split[PBKDF2Index]);
			byte[] testHash = PBKDF2(password, salt, iterations, hash.Length);

			return SlowEquals(hash, testHash);
		}

		/// <summary>
		/// Compares two byte arrays in length-constant time.
		/// This comparison method is used so that password hashes cannot be extracted from online systems using a timing attack and then attacked off-line.
		/// </summary>
		/// <param name="a">The first byte array.</param>
		/// <param name="b">The second byte array.</param>
		/// <returns>True if both byte arrays are equal. False otherwise.</returns>
		static bool SlowEquals(byte[] a, byte[] b)
		{
			uint diff = (uint)a.Length ^ (uint)b.Length;

			for (int i = 0; i < a.Length && i < b.Length; i++)
			{
				diff |= (uint)(a[i] ^ b[i]);
			}

			return diff == 0;
		}

		/// <summary>
		/// Computes the PBKDF2-SHA1 hash of a password.
		/// </summary>
		/// <param name="password">The password to hash.</param>
		/// <param name="salt">The salt.</param>
		/// <param name="iterations">The PBKDF2 iteration count.</param>
		/// <param name="outputBytes">The length of the hash to generate, in bytes.</param>
		/// <returns>A hash of the password.</returns>
		static byte[] PBKDF2(string password, byte[] salt, int iterations, int outputBytes)
		{
			Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt) { IterationCount = iterations };

			return pbkdf2.GetBytes(outputBytes);
		}
	}
}