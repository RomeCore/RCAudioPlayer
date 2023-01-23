using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RCAudioPlayer.WPF.Files
{
	public static class State
	{
		public static readonly string FileName = Core.Files.UserFolder + "\\state.json";

		private static Dictionary<string, object> dictionary;

		static State()
		{
			Dictionary<string, object>? data = null;

			if (File.Exists(FileName))
			{
				var fileData = File.ReadAllText(FileName);
				data = JsonConvert.DeserializeObject<Dictionary<string, object>>(fileData);
			}

			if (data == null)
				dictionary = new Dictionary<string, object>();
			else
				dictionary = data;
		}

		public static object Get(string key)
		{
			return dictionary[key];
		}

		public static T Get<T>(string key, T @default)
		{
			if (dictionary.TryGetValue(key, out var obj))
				if ((obj = Convert.ChangeType(obj, typeof(T))) != null)
					return (T)obj;
			return @default;
		}

		public static void Set(string key, object obj)
		{
			dictionary[key] = obj;
		}

		static public void Save()
		{
			var fileData = JsonConvert.SerializeObject(dictionary, Formatting.Indented);
			File.WriteAllText(FileName, fileData);
		}
	}
}