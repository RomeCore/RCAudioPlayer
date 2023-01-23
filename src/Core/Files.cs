using System;
using System.IO;

namespace RCAudioPlayer.Core
{
	static public class Files
	{
		public static string UserFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + '\\' + "RC Audio Player";

	}
}