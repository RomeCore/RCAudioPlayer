using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using RCAudioPlayer.Core;
using RCAudioPlayer.Core.Players;
using RCAudioPlayer.WPF.Players.Sub;
using RCAudioPlayer.WPF.Players.Editors;

namespace RCAudioPlayer.WPF.Players
{
	static public class PlayerControlDictionary
	{
		static private Dictionary<Type, ConstructorInfo> Specials { get; }
		static private Dictionary<Type, ConstructorInfo> SubSpecials { get; }
		static private Dictionary<Type, ConstructorInfo> EditorSpecials { get; }

		static PlayerControlDictionary()
		{
			var playerControlType = typeof(PlayerControl);
			Specials = (from type in PluginManager.Types
						where playerControlType.IsAssignableFrom(type)
						let attribute = type.GetCustomAttribute<PlayerControlAttribute>()
						where attribute != null
						let constructor = type.GetConstructor(new Type[] { attribute.PlayerType })
						where constructor != null
						select (attribute.PlayerType, constructor))
					   .OrderByDescending(t => t.Item1.GetInheritanceHierarchy().Count)
					   .ToDictionary(k => k.Item1, s => s.constructor);

			var playerSubControlType = typeof(PlayerSubControl);
			SubSpecials = (from type in PluginManager.Types
						   where playerSubControlType.IsAssignableFrom(type)
						   let attribute = type.GetCustomAttribute<PlayerSubControlAttribute>()
						   where attribute != null
						   let constructor = type.GetConstructor(new Type[] { attribute.PlayerType })
						   where constructor != null
						   select (attribute.PlayerType, constructor))
						  .OrderByDescending(t => t.Item1.GetInheritanceHierarchy().Count)
						  .ToDictionary(k => k.Item1, s => s.constructor);

			var playerEditorDialogType = typeof(PlayerEditorDialog);
			var stringType = typeof(string);
			EditorSpecials = (from type in PluginManager.Types
							  where playerEditorDialogType.IsAssignableFrom(type)
							  let attribute = type.GetCustomAttribute<PlayerEditorDialogAttribute>()
							  where attribute != null
							  let constructor = type.GetConstructor(new Type[] { stringType }) ?? type.GetConstructor(Type.EmptyTypes)
							  where constructor != null
							  select (attribute.PlayerType, constructor))
							 .OrderByDescending(t => t.Item1.GetInheritanceHierarchy().Count)
							 .ToDictionary(k => k.Item1, s => s.constructor);
		}

		static private PlayerControl Get(ConstructorInfo constructor, Player player)
		{
			return (PlayerControl)constructor.Invoke(new object[] { player });
		}

		static private PlayerSubControl GetSub(ConstructorInfo constructor, Player player)
		{
			return (PlayerSubControl)constructor.Invoke(new object[] { player });
		}

		static private PlayerEditorDialog GetEditor(ConstructorInfo constructor, string startingResult)
		{
			if (constructor.GetParameters().Length == 1)
				return (PlayerEditorDialog)constructor.Invoke(new object[] { startingResult });
			else
				return (PlayerEditorDialog)constructor.Invoke(null);
		}

		static public PlayerControl? GetFor(Player player)
		{
			var playerType = player.GetType();

			if (Specials.TryGetValue(playerType, out var constructor))
				return Get(constructor, player);

			foreach (var special in Specials)
				if (special.Key.IsAssignableFrom(playerType))
					return Get(special.Value, player);

			return null;
		}

		static public PlayerSubControl? GetSubFor(Player player)
		{
			var playerType = player.GetType();

			if (SubSpecials.TryGetValue(playerType, out var constructor))
				return GetSub(constructor, player);

			foreach (var special in SubSpecials)
				if (special.Key.IsAssignableFrom(playerType))
					return GetSub(special.Value, player);

			return null;
		}

		static public PlayerEditorDialog GetEditorFor(Type playerType, string startingResult = "")
		{
			if (!typeof(Player).IsAssignableFrom(playerType))
				throw new ArgumentException("This type is not player type!", nameof(playerType));

			if (EditorSpecials.TryGetValue(playerType, out var constructor))
				return GetEditor(constructor, startingResult);

			foreach (var special in EditorSpecials)
				if (special.Key.IsAssignableFrom(playerType))
					return GetEditor(special.Value, startingResult);

			return new GenericEditorDialog(startingResult);
		}
	}
}