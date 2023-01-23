using System.IO;
using System.Net.Http;

namespace RCAudioPlayer.Core.Data
{
	// Utility tool for getting strings, bytes or streams from uris
	public static class NetClient
	{
		public static HttpClient Http { get; }

		static NetClient()
		{
			Http = new HttpClient();
		}

		public static string GetString(string uri)
		{
			if (File.Exists(uri))
				return File.ReadAllText(uri);
			return DownloadString(uri);
		}

		public static byte[] Get(string uri)
		{
			if (File.Exists(uri))
				return File.ReadAllBytes(uri);
			return Download(uri);
		}

		public static Stream GetStream(string uri)
		{
			if (File.Exists(uri))
				return File.OpenRead(uri);
			return DownloadStream(uri);
		}

		public static string DownloadString(string uri)
		{
			return Http.GetStringAsync(uri).Result;
		}

		public static byte[] Download(string uri)
		{
			return Http.GetByteArrayAsync(uri).Result;
		}

		public static Stream DownloadStream(string uri)
		{
			return Http.GetStreamAsync(uri).Result;
		}
	}
}