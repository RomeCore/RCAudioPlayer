using System;
using System.IO;

namespace RCAudioPlayer.Core
{
	public static class Log
	{
		static Log()
		{
		}

		static private void Write(string message)
		{
			try
			{
				File.AppendAllText("log.txt", message);
			}
			catch { }
			Console.WriteLine(message);
		}

		static public void Exception(Exception exception, string exceptionDesc = "")
		{
			if (!string.IsNullOrWhiteSpace(exceptionDesc))
				exceptionDesc = " " + exceptionDesc;
			string message = $"[{DateTime.Now}] Exception was occured{exceptionDesc}: {exception.Message}\n" +
				$"\t{exception.StackTrace}\n";
			Write(message);
		}

		static public void Warning(string message)
		{
			message = $"[{DateTime.Now}] Warning: {message}";
			Write(message);
		}

		static public void Info(string message)
		{
			message = $"[{DateTime.Now}] Info: {message}";
			Write(message);
		}
	}
}