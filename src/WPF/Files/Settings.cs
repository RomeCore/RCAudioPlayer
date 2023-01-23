using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RCAudioPlayer.WPF.Translation;

namespace RCAudioPlayer.WPF.Files
{
	public static class Settings
	{
		static private PropertyInfo[] properies = typeof(Settings).GetProperties();
		public static readonly string FileName = Core.Files.UserFolder + "\\settings.json";

		public static string Language { get => TranslationDictionary.LoadedLanguage; set => TranslationDictionary.LoadLanguage(value); }

		public static Theme Theme { get => App.Theme; set => App.Theme = value; }
		public static Color PrimaryThemeColor { get => App.PrimaryColor; set => App.PrimaryColor = value; }
		public static Color SecondaryThemeColor { get => App.SecondaryThemeColor; set => App.SecondaryThemeColor = value; }

		// Loads settings from file stored in AppData
		public static void Load()
		{
			// Load settings from file (if it exists)
			if (File.Exists(FileName))
			{
				var fileData = File.ReadAllText(FileName);
				var data = JsonConvert.DeserializeObject<JContainer>(fileData);

				// Algorithm to set properties from json data to this static class
				foreach (var property in properies)
				{
					var value = data?[property.Name]?.ToObject(property.PropertyType);
					if (value != null)
						property.SetValue(null, value);
				}

				App.UpdateThemeConfig();
			}
			// Set default settings
			else
			{
				App.Configure(Color.FromRgb(60, 120, 200), Color.FromRgb(200, 200, 200), Theme.Light);
				TranslationDictionary.LoadDefault();
			}
		}

		// Saves settings to json file
		public static void Save()
		{
			var settingsData = properies.ToDictionary(x => x.Name, x => x.GetValue(null));
			var fileData = JsonConvert.SerializeObject(settingsData, Formatting.Indented);
			File.WriteAllText(FileName, fileData);
		}
	}
}