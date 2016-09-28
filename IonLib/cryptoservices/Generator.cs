﻿using System;

namespace IonLib.cryptoservices
{
	public static class Generator
	{
		const int Size = 16;

		public static string NextIVForAES()
		{
			return Guid.NewGuid().ToString("n").Substring(0, Size);
		}
	}
}