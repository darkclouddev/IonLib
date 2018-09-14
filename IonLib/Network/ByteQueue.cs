using System;

namespace IonLib.Network
{
	public class ByteQueue
	{
		public int Length { get; private set; }
		public byte[] BaseBuffer { get; set; }

		public int Head { get; private set; }
		public int Tail { get; private set; }

		public int Size => Length;

		public ByteQueue()
		{
			BaseBuffer = new byte[2048];
		}

		public void Clear()
		{
			Head = 0;
			Tail = 0;
			Length = 0;
		}

		public void SetCapacity(int capacity, bool alwaysNewBuffer)
		{
			if (!alwaysNewBuffer)
			{
				if (BaseBuffer is null || BaseBuffer.Length < capacity)
				{
					BaseBuffer = new byte[capacity];
				}
			}
			else
			{
				byte[] newBuffer = new byte[capacity];

				if (Length > 0)
				{
					if (Head < Tail)
					{
						Buffer.BlockCopy(BaseBuffer, Head, newBuffer, 0, Length);
					}
					else
					{
						Buffer.BlockCopy(BaseBuffer, Head, newBuffer, 0, BaseBuffer.Length - Head);
						Buffer.BlockCopy(BaseBuffer, 0, newBuffer, BaseBuffer.Length - Head, Tail);
					}
				}

				BaseBuffer = newBuffer;
			}

			Head = 0;
			Tail = Length;
		}

		public byte[] GetPacketData()
		{
			if (Length >= 1)
				return new[]
				{
					BaseBuffer[Head],
					BaseBuffer[1]
				};

			return new byte[] { 0x46 }; //70
		}

		public int CopyAll(byte[] buffer)
		{
			if (Head < Tail)
			{
				Buffer.BlockCopy(BaseBuffer, Head, buffer, 0, Length);
			}
			else
			{
				int rightLength = (BaseBuffer.Length - Head);

				if (rightLength >= Length)
				{
					Buffer.BlockCopy(BaseBuffer, Head, buffer, 0, Length);
				}
				else
				{
					Buffer.BlockCopy(BaseBuffer, Head, buffer, 0, rightLength);
					Buffer.BlockCopy(BaseBuffer, 0, buffer, 0 + rightLength, Length - rightLength);
				}
			}

			return Length;
		}

		public int Dequeue(byte[] buffer, int offset, int size)
		{
			if (size > Length)
				size = Length;

			if (size == 0)
				return 0;

			if (Head < Tail)
			{
				Buffer.BlockCopy(BaseBuffer, Head, buffer, offset, size);
			}
			else
			{
				int rightLength = (BaseBuffer.Length - Head);

				if (rightLength >= size)
				{
					Buffer.BlockCopy(BaseBuffer, Head, buffer, offset, size);
				}
				else
				{
					Buffer.BlockCopy(BaseBuffer, Head, buffer, offset, rightLength);
					Buffer.BlockCopy(BaseBuffer, 0, buffer, offset + rightLength, size - rightLength);
				}
			}

			Head = (Head + size) % BaseBuffer.Length;
			Length -= size;

			if (Length != 0)
				return size;

			Head = 0;
			Tail = 0;

			return size;
		}

		public void Enqueue(byte[] buffer, int offset, int size)
		{
			if ((Length + size) > BaseBuffer.Length)
				SetCapacity((Length + size + 2047) & ~2047, true);

			if (Head < Tail)
			{
				int rightLength = (BaseBuffer.Length - Tail);

				if (rightLength >= size)
				{
					Buffer.BlockCopy(buffer, offset, BaseBuffer, Tail, size);
				}
				else
				{
					Buffer.BlockCopy(buffer, offset, BaseBuffer, Tail, rightLength);
					Buffer.BlockCopy(buffer, offset + rightLength, BaseBuffer, 0, size - rightLength);
				}
			}
			else
			{
				Buffer.BlockCopy(buffer, offset, BaseBuffer, Tail, size);
			}

			Tail = (Tail + size) % BaseBuffer.Length;
			Length += size;
		}
	}
}