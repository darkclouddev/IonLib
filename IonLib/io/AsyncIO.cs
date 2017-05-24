using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace IonLib.io
{
	public static class AsyncIO
	{
		const int BufferSize = 4096;
		const FileOptions Options = FileOptions.Asynchronous | FileOptions.SequentialScan;

		public async static Task<string> ReadAllTextAsync(string path)
		{
			return await ReadAllTextAsync(path, Encoding.UTF8);
		}

		public async static Task<string> ReadAllTextAsync(string path, Encoding encoding)
		{
			string text;

			using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize, Options))
			{
				using (var reader = new StreamReader(stream, encoding))
				{
					text = await reader.ReadToEndAsync();
				}
			}

			return text;
		}

		public async static Task WriteAllTextAsync(string path, string text)
		{
			byte[] encodedText = Encoding.UTF8.GetBytes(text);

			FileMode mode = File.Exists(path) ? FileMode.Create : FileMode.CreateNew;

			using (var fs = new FileStream(path, mode, FileAccess.Write, FileShare.None, 4096, true))
			{
				await fs.WriteAsync(encodedText, 0, encodedText.Length);
			}
		}
	}
}