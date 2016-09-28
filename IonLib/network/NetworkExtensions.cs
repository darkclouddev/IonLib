using System;
using System.Net.Sockets;
using System.Text;

namespace IonLib.network
{
	public static class NetworkExtensions
	{
		public static int SendString(this Socket socket, string str, Encoding encoding, bool addNewlineChar = true)
		{
			if (addNewlineChar) str += "\n";
			byte[] buffer = encoding.GetBytes(str);

			return socket.Send(buffer, buffer.Length, SocketFlags.None);
		}

		public static bool IsConnected(this Socket socket)
		{
			return !((socket.Poll(1000, SelectMode.SelectRead) && (socket.Available == 0)) || !socket.Connected);
		}

		public static void Connect(this Socket socket, string host, int port, TimeSpan timeout)
		{
			AsyncConnect(socket, (s, a, o) => s.BeginConnect(host, port, a, o), timeout);
		}

		static void AsyncConnect(Socket socket, Func<Socket, AsyncCallback, object, IAsyncResult> connect, TimeSpan timeout)
		{
			IAsyncResult asyncResult = connect(socket, null, null);

			if (!asyncResult.AsyncWaitHandle.WaitOne(timeout))
			{
				try
				{
					socket.EndConnect(asyncResult);
				}
				catch (SocketException)
				{
					throw;
				}
				catch (ObjectDisposedException)
				{
					throw;
				}
			}
		}
	}
}