using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RCAudioPlayer.WPF.Translation
{
	// Provides methods to load translations and use them
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

			if (Directory.Exists(folderPath))
			{
				List<Dictionary<string, string>> dictionaries = new List<Dictionary<string, string>>();

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

		static public void LoadDefault()
		{
			var languages = Directory.EnumerateDirectories(Folder).Select(s => Path.GetDirectoryName(s)).ToList();

			var currentCultureDisplay = System.Globalization.CultureInfo.InstalledUICulture.DisplayName;
			if (languages.Contains(currentCultureDisplay))
			{
				LoadLanguage(currentCultureDisplay);
				return;
			}

			if (!currentCultureDisplay.Contains('('))
				return;
			var shortName = currentCultureDisplay.Substring(0, currentCultureDisplay.IndexOf('(')).Trim();
			if (languages.Contains(shortName))
			{
				LoadLanguage(shortName);
				return;
			}

			if (languages.Contains("English"))
				LoadLanguage("English");
		}
	}
}