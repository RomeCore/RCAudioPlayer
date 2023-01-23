using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using RCAudioPlayer.Core.Effects;
using RCAudioPlayer.Core.Effects.Attributes;

namespace RCAudioPlayer.Core
{
	public static class EffectDictionary
	{
		public class PresetCollection
		{
			public static readonly string Folder = Files.UserFolder + "\\presets";
			public const string FileExtension = ".txt";
			public const string FilenamePattern = "*" + FileExtension;
			public const string CustomPresetName = "custom";

			private readonly Dictionary<string, string> _presets;

			public string Path { get; }
			public IReadOnlyDictionary<string, string> Presets { get => _presets; }

			static PresetCollection()
			{
				Directory.CreateDirectory(Folder);
			}

			public PresetCollection(string folder)
			{
				Path = System.IO.Path.Combine(Folder, folder);

				Directory.CreateDirectory(Path);
				_presets = Directory.EnumerateFiles(Path, FilenamePattern)
					.ToDictionary(k => GetName(k), v => File.ReadAllText(v));
			}

			private string GetFilename(string name)
			{
				return System.IO.Path.Combine(Path, name + FileExtension);
			}

			private string GetName(string filename)
			{
				return System.IO.Path.GetFileNameWithoutExtension(filename);
			}

			public void Select(AudioEffect effect, string presetName, string defaultPreset)
			{
				if (presetName == CustomPresetName)
				{
					effect.LoadPreset(CustomPresetName, effect.CustomPreset);
					return;
				}
				if (Presets.TryGetValue(presetName, out string? preset))
				{
					effect.LoadPreset(presetName, preset);
					return;
				}
				effect.LoadPreset(CustomPresetName, defaultPreset);
			}

			public void Set(string name, string preset)
			{
				File.WriteAllText(GetFilename(name), preset);
				_presets[name] = preset;
			}

			public void Remove(string name)
			{
				if (Presets.ContainsKey(name))
				{
					_presets.Remove(name);
					File.Delete(GetFilename(name));
				}
			}
		}

		public class Data
		{
			public Type Type { get; }
			public EffectAttribute Attribute { get; }
			public IReadOnlyDictionary<PropertyInfo, PropertyAttribute> Properties { get; }
			public PresetCollection Presets { get; }

			public Data(Type type, EffectAttribute attribute)
			{
				Type = type;
				Attribute = attribute;
				Properties = ExtractProperies(type);
				Presets = new PresetCollection(attribute.Id);
			}

			public static IReadOnlyDictionary<PropertyInfo, PropertyAttribute> ExtractProperies(Type type)
			{
				return (from property in type.GetProperties()
					let attribute = property.GetCustomAttribute<PropertyAttribute>(true)
					where attribute != null
					select (property, attribute))
					.OrderBy(s => s.attribute.Order)
					.ToDictionary(k => k.property, s => s.attribute);
			}
		}

		public static IReadOnlyList<Data> Dict { get; }

		static EffectDictionary()
		{
			var effectType = typeof(AudioEffect);
			Dict = (from type in PluginManager.Types
				let attribute = type.GetCustomAttribute<EffectAttribute>(false)
				where attribute != null
				select new Data(type, attribute)).ToList();
		}

		static public Data? Find<TEffect>() where TEffect : AudioEffect
		{
			return FindByType(typeof(TEffect));
		}
		
		static public Data? FindByType(Type type)
		{
			foreach (var effectData in Dict)
				if (effectData.Type == type)
					return effectData;
			return null;
		}

		static public Data? FindById(string id)
		{
			foreach (var effectData in Dict)
				if (effectData.Attribute.Id == id)
					return effectData;
			return null;
		}

		static public AudioEffect? CreateEffectInstance(Data data)
		{
			return Activator.CreateInstance(data.Type) as AudioEffect;
		}

		static public AudioEffect? CreateDefaultEffectInstance(Data data)
		{
			var effect = CreateEffectInstance(data);
			if (effect != null)
				effect.LoadPreset(PresetCollection.CustomPresetName, data.Attribute.DefaultPreset);
			return effect;
		}

		static public AudioEffect? CreateEffectInstance(Type type)
		{
			return Activator.CreateInstance(type) as AudioEffect;
		}

		static public AudioEffect? CreateEffectInstance(string id)
		{
			var data = FindById(id);
			if (data != null)
				return Activator.CreateInstance(data.Type) as AudioEffect;
			return null;
		}

		static public PresetCollection? GetPresetsFor(Type type)
		{
			return FindByType(type)?.Presets;
		}
	}
}