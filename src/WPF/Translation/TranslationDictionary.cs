using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RCAudioPlayer.WPF.Translation
{
	static public class TranslationDictionary
	{
		public const string Folder = "lang";

		static private Dictionary<string, string>? dictionary;
		static public string LoadedLanguage { get; private set; }
		static public event EventHandler LanguageChanged = (s, e) => { };

		static TranslationDictionary()
		{
			LoadedLanguage = string.Empty;
		}

		static public string Get(string key)
		{
			if (dictionary?.TryGetValue(key, out var value) ?? false)
				return value;
			return key;
		}

		static public void LoadLanguage(string langName)
		{
			var folderPath = Path.Combine(Folder, langName);
			List<Dictionary<string, string>> dictionaries = new List<Dictionary<string, string>>();

			if (Directory.Exists(folderPath))
			{
				foreach (var jsonFile in Directory.EnumerateFiles(folderPath, "*.json"))
				{
					var fileData = File.ReadAllText(jsonFile);
					var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileData);
					if (dict != null)
						dictionaries.Add(dict);
				}

				dictionary = dictionaries.SelectMany(dict => dict)
							 .ToLookup(pair => pair.Key, pair => pair.Value)
							 .ToDictionary(group => group.Key, group => group.First());
				LanguageChanged(null, EventArgs.Empty);
				LoadedLanguage = langName;
			}
		}
	}
}