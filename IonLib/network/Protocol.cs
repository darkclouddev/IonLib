using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using IonLib.cryptoservices;

namespace IonLib.network
{
	/// <summary>
	/// Protocol container class. Provides methods for packet processing, receiving and sending.
	/// </summary>
	public static class Protocol
	{
		public const string EmptyIV = "0000000000000000";

		/* Process description:
		 * 1) Insert data
		 * 2) Insert size of data (4 bytes int)
		 * 3) Insert packet type (4 bytes int)
		 * 4) Insert crc32 of type+size+data (4 bytes uint)
		 * 5) Insert empty IV (16 bytes string)
		 */
		public static byte[] CreatePacket(byte[] serializedPacket, int packetType)
		{
			byte[] packetBytes = serializedPacket; //Insert data

			packetBytes = InsertBytesAtIndex(0, BitConverter.GetBytes(packetBytes.Length), packetBytes); //+size
			packetBytes = InsertBytesAtIndex(0, BitConverter.GetBytes(packetType), packetBytes); //+type
			packetBytes = InsertBytesAtIndex(0, BitConverter.GetBytes(CRC32Operation.ComputeCRC32(packetBytes)), packetBytes); //+crc
			packetBytes = InsertBytesAtIndex(0, Encoding.ASCII.GetBytes(EmptyIV), packetBytes); //+empty iv

			return packetBytes;
		}

		/* Process description:
		 * 1) Insert data
		 * 2) Insert size of data (4 bytes int)
		 * 3) Insert packet type (4 bytes int)
		 * 4) Insert crc32 of type+size+data (4 bytes uint)
		 * 5) Encrypt entire array w/ AES256
		 * 6) Insert size of encrypted data (4 bytes int)
		 * 7) Insert IV (16 bytes string)
		 */
		public static byte[] CreateEncryptedPacket(byte[] serializedPacket, int packetType, byte[] key, byte[] iv)
		{
			byte[] finalBytes = serializedPacket; //Insert data

			finalBytes = InsertBytesAtIndex(0, BitConverter.GetBytes(finalBytes.Length), finalBytes); //+size
			finalBytes = InsertBytesAtIndex(0, BitConverter.GetBytes(packetType), finalBytes); //+type
			finalBytes = InsertBytesAtIndex(0, BitConverter.GetBytes(CRC32Operation.ComputeCRC32(finalBytes)), finalBytes); //+crc
			finalBytes = AES256.Encrypt(finalBytes, key, iv); //+enc
			Console.WriteLine($"Encrypted data: {BitConverter.ToString(finalBytes)}");
			finalBytes = InsertBytesAtIndex(0, BitConverter.GetBytes(finalBytes.Length), finalBytes);  //+encSize
			finalBytes = InsertBytesAtIndex(0, iv, finalBytes); //+iv

			return finalBytes;
		}
		public static byte[] CreateEncryptedPacket(byte[] serializedPacket, int packetType, byte[] key, string iv)
		{
			return CreateEncryptedPacket(serializedPacket, packetType, key, Encoding.UTF8.GetBytes(iv));
		}

		public static bool IsValidCRC32(uint crc, byte[][] packetParts)
		{
			byte[] total = new byte[packetParts.Sum(parts => parts.Length)];
			int offset = 0;

			foreach (byte[] part in packetParts)
			{
				Array.Copy(part, 0, total, offset, part.Length);
				offset += part.Length;
			}

			return crc == CRC32Operation.ComputeCRC32(total);
		}

		public static byte[] InsertBytesAtIndex(int index, byte[] insert, byte[] data)
		{
			byte[] result = new byte[data.Length + insert.Length];

			if (index == 0)
			{
				Array.Copy(insert, 0, result, 0, insert.Length);
				Array.Copy(data, 0, result, insert.Length, data.Length);

				return result;
			}

			Array.Copy(data, 0, result, 0, index);
			Array.Copy(insert, 0, result, index, insert.Length);
			Array.Copy(data, index, result, insert.Length + index, data.Length - index);

			return result;
		}

		public static byte[] ReadBytesAtIndex(int index, int headerSize, byte[] data)
		{
			byte[] temp = new byte[headerSize];
			Array.Copy(data, index, temp, 0, headerSize);

			return temp;
		}

		public static int GetHeaderSize(HeaderType type)
		{
			switch (type)
			{
				case HeaderType.EncryptedSize: //int (4)
				case HeaderType.CRC: //uint (4)
				case HeaderType.DataType: //int (4)
				case HeaderType.DataSize: return 4; //int (4)
				case HeaderType.IV: return 16; //string (16)

				default: return 0; //fail
			}
		}

		/// <summary>
		/// Client-side only! Serialization method.
		/// </summary>
		/// <typeparam name="T">Data type (usually packet class)</typeparam>
		/// <param name="packet">Byte array of data to serialize</param>
		/// <returns></returns>
		public static byte[] Serialize<T>(ref T packet) where T : struct
		{
			int size = Marshal.SizeOf(packet);
			byte[] serializedData = new byte[size];
			IntPtr dataPointer = Marshal.AllocHGlobal(size);

			Marshal.StructureToPtr(packet, dataPointer, true);
			Marshal.Copy(dataPointer, serializedData, 0, size);
			Marshal.FreeHGlobal(dataPointer);

			return serializedData;
		}

		/// <summary>
		/// Client-side only! Deserialization method.
		/// </summary>
		/// <typeparam name="T">Data type (usually packet class)</typeparam>
		/// <param name="bytes">Byte array of data to deserialize</param>
		/// <returns></returns>
		public static T Deserialize<T>(byte[] bytes) where T : struct
		{
			GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
			T packet = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
			handle.Free();

			return packet;
		}
	}

	public enum HeaderType
	{
		IV,
		EncryptedSize,
		CRC,
		DataType,
		DataSize,
	}
}