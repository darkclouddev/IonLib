using System;
using System.Text;

namespace IonLib.util
{
	public static class Extensions
	{
		/// <summary>
		/// Checks if number is in given range
		/// </summary>
		/// <param name="number">Number to check</param>
		/// <param name="from">From number</param>
		/// <param name="to">To number</param>
		/// <param name="inclusive">Inclusive check for both numbers</param>
		/// <returns>True if number is in given range</returns>
		public static bool IsInRange(this int number, int from, int to, bool inclusive)
		{
			return inclusive ? from <= number && number <= to : from < number && number < to;
		}

		public static int ToInt32(byte[] buffer)
		{
			if (buffer == null) throw new ArgumentNullException("Buffer cannot be null!");

			return BitConverter.ToInt32(buffer, 0);
		}

		public static string ToStringIV(byte[] buffer)
		{
			if (buffer == null) throw new ArgumentNullException("Buffer cannot be null!");

			return Encoding.ASCII.GetString(buffer);
		}

		public static string ToString(byte[] buffer)
		{
			if (buffer == null) throw new ArgumentNullException("Buffer cannot be null!");

			return Encoding.UTF8.GetString(buffer);
		}

		public static uint ToUInt32(byte[] buffer)
		{
			if (buffer == null) throw new ArgumentNullException("Buffer cannot be null!");

			return BitConverter.ToUInt32(buffer, 0);
		}
	}
}