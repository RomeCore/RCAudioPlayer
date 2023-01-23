using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using RCAudioPlayer.Core.Players;
using System.Threading.Tasks;

namespace RCAudioPlayer.Core
{
	public class PlayerManager : IDisposable
	{
		public static readonly string Folder = Files.UserFolder + "\\players";
		public const string FileExtension = ".txt";
		public const string FilenamePattern = "*" + FileExtension;

		static public string GetFilename(string name)
		{
			return Path.Combine(Folder, name + FileExtension);
		}

		static public string GetName(string filename)
		{
			return Path.GetFileNameWithoutExtension(filename);
		}

		public class PlayerData
		{
			public string Filename { get; }
			public string Name { get; }
			public string TypeName { get; }
			public TypeData TypeData { get; }

			public PlayerData(string filename) : this(filename, GetName(filename))
			{
			}

			public PlayerData(string filename, string name)
			{
				Filename = filename;
				Name = name;

				var reader = new StreamReader(filename);
				var line = reader.ReadLine();

				if (line != null)
					TypeName = line;
				else
					TypeName = string.Empty;
				TypeData = Types[TypeName];
			}
		}

		public class TypeData
		{
			public Type Type { get; }
			public PlayerAttribute Attribute { get; }
			public ConstructorInfo Constructor { get; }

			public TypeData(Type type, PlayerAttribute attribute, ConstructorInfo constructor)
			{
				Type = type;
				Attribute = attribute;
				Constructor = constructor;
			}
		}

		public class ChangedArgs
		{
			public PlayerData Data { get; }
			public Player Player { get; }

			public ChangedArgs(PlayerData data, Player player)
			{
				Data = data;
				Player = player;
			}
		}

		public class ListUpdatedArgs
		{
			public IEnumerable<string> List { get; private set; }

			public ListUpdatedArgs(IEnumerable<string> list)
			{
				List = list;
			}
		}

		private static readonly Dictionary<string, PlayerData> playerFiles;

		public static IReadOnlyDictionary<string, TypeData> Types { get; }
		public static IEnumerable<string> TypesList => Types.Keys;
		public static IReadOnlyDictionary<string, PlayerData> Players => playerFiles;
		public static IEnumerable<string> PlayersList => playerFiles.Keys;

		static public async Task<Player?> Create(StreamReader reader, string name, PlayerMaster master)
		{
			string? typeName;
			typeName = reader.ReadLine();

			if (typeName != null && Types.TryGetValue(typeName, out var typeData))
			{
				var player = typeData.Constructor.Invoke(new object[] { master, name }) as Player;
				if (player != null)
				{
					await player.Load(reader);
					reader.Close();
					return player;
				}
			}
			reader.Close();
			return null;
		}

		static PlayerManager()
		{
			var playerType = typeof(Player);
			var constructorArgs = new Type[] { typeof(PlayerMaster), typeof(string) };
			Types = (from type in PluginManager.Types
						   where playerType.IsAssignableFrom(type)
						   let attribute = type.GetCustomAttribute<PlayerAttribute>()
						   where attribute != null
						   let constructor = type.GetConstructor(constructorArgs)
						   where constructor != null
						   select (type, attribute, constructor)).ToDictionary(k => k.attribute.TypeName, s => new TypeData(s.type, s.attribute, s.constructor));

			Directory.CreateDirectory(Folder);
			playerFiles = new Dictionary<string, PlayerData>();
			foreach (var file in Directory.EnumerateFiles(Folder, FilenamePattern))
			{
				try
				{
					playerFiles.Add(GetName(file), new PlayerData(file));
				}
				catch (Exception exc)
				{
					Log.Exception(exc, "while loading player data");
				}
			}
		}

		public PlayerMaster Master { get; }
		public Player? Current { get; private set; }
		public string CurrentName { get; private set; }

		public event EventHandler<ChangedArgs> OnChanged = (s, e) => { };
		public event EventHandler<ListUpdatedArgs> OnListUpdated = (s, e) => { };

		public PlayerManager()
		{
			Master = new PlayerMaster();
			CurrentName = string.Empty;
		}

		public void Dispose()
		{
			Unload();
			Master.Dispose();
		}

		public async Task<bool> Load(string playerName)
		{
			SaveCurrent();
			if (playerFiles.TryGetValue(playerName, out var data))
			{
				var player = await Create(playerName);
				if (player != null)
				{
					Current = player;
					CurrentName = playerName;
					OnChanged(this, new ChangedArgs(data, Current));
					return true;
				}
			}
			return false;
		}

		public void SaveCurrent()
		{
			if (Current != null && PlayersList.Contains(CurrentName))
			{
				var filename = GetFilename(CurrentName);
				var stream = File.Create(filename);
				var writer = new StreamWriter(stream);

				var currentType = Current.GetType();
				var playerType = Types.First(s => s.Value.Type == currentType);

				writer.WriteLine(playerType.Value.Attribute.TypeName);
				Current.Save(writer);
				writer.Close();
			}
		}

		public void Save()
		{
			SaveCurrent();
			Master.Save();
		}

		public Task<Player?> CreateFromFile(string filename)
		{
			return Create(new StreamReader(filename), GetName(filename), Master);
		}

		public Task<Player?> Create(string name)
		{
			return CreateFromFile(GetFilename(name));
		}

		public void Unload()
		{
			SaveCurrent();
			Current = null;
		}

		public bool Add(string name, string className, string value = "")
		{
			if (name.Contains('\\') || name.Contains('/'))
				return false;
			var filename = GetFilename(name);

			if (!playerFiles.ContainsKey(name))
			{
				var writer = new StreamWriter(filename);
				writer.WriteLine(className);
				writer.Write(value);
				writer.Close();

				playerFiles.Add(name, new PlayerData(filename));
				OnListUpdated(this, new ListUpdatedArgs(PlayersList));
				return true;
			}
			return false;
		}

		public bool Remove(string name)
		{
			string filename = GetFilename(name);

			if (File.Exists(filename))
			{
				playerFiles.Remove(name);
				File.Delete(filename);
				OnListUpdated(this, new ListUpdatedArgs(PlayersList));
				return true;
			}
			return false;
		}

		public string ExtractContent(string name)
		{
			var filename = GetFilename(name);
			var reader = new StreamReader(filename);
			var line = reader.ReadLine();
			if (line != null)
			{
				var content = reader.ReadToEnd();
				reader.Close();
				return content;
			}
			reader.Close();
			return string.Empty;
		}

		public void ChangeContent(string name, string content)
		{
			var filename = GetFilename(name);
			var reader = new StreamReader(filename);

			var className = reader.ReadLine();
			reader.Close();
			if (className != null)
			{
				var writer = new StreamWriter(filename);
				writer.WriteLine(className);
				writer.Write(content);
				writer.Close();
			}
		}

		public bool Move(string originalName, string targetName)
		{
			string originalFilename = GetFilename(originalName);

			if (File.Exists(originalFilename) && originalName != targetName)
			{
				var targetFilename = GetFilename(targetName);

				File.Move(originalFilename, targetFilename);
				playerFiles.Remove(originalName);
				playerFiles.Add(targetName, new PlayerData(targetFilename));

				if (originalName == Current?.Name)
					Current.Name = targetName;

				OnListUpdated(this, new ListUpdatedArgs(PlayersList));
				return true;
			}
			return false;
		}
	}
}