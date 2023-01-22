using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using RCAudioPlayer.Core;
using RCAudioPlayer.Core.Players;
using RCAudioPlayer.WPF.Playables.Sub;

namespace RCAudioPlayer.WPF.Playables
{
    static public class PlayableControlDictionary
	{
		static private Dictionary<Type, ConstructorInfo> Specials { get; }
		static private Dictionary<Type, ConstructorInfo> SubSpecials { get; }

		static PlayableControlDictionary()
		{
			var playableControlType = typeof(PlayableControl);
			var playerType = typeof(Player);
			Specials = (from type in PluginManager.Types
						where playableControlType.IsAssignableFrom(type)
						let attribute = type.GetCustomAttribute<PlayableControlAttribute>()
						where attribute != null
						let constructor = type.GetConstructor(new Type[] { attribute.PlayableType, playerType }) ?? type.GetConstructor(new Type[] { attribute.PlayableType })
						where constructor != null
						select (attribute.PlayableType, constructor))
					   .ToDictionary(k => k.Item1, s => s.constructor);

            var playableSubControlType = typeof(PlayableSubControl);
            SubSpecials = (from type in PluginManager.Types
						   where playableSubControlType.IsAssignableFrom(type)
						   let attribute = type.GetCustomAttribute<PlayableSubControlAttribute>()
						   where attribute != null
						   let constructor = type.GetConstructor(new Type[] { attribute.PlayableType, playerType }) ?? type.GetConstructor(new Type[] { attribute.PlayableType })
						   where constructor != null
						   select (attribute.PlayableType, constructor))
						  .ToDictionary(k => k.Item1, s => s.constructor);
		}

		static private T? GetFor<T>(Dictionary<Type, ConstructorInfo> dict, IPlayable playable, Player player) where T : class
		{
            if (dict.TryGetValue(playable.GetType(), out var constructor))
			{
				if (constructor.GetParameters().Length == 2)
                    return (T)constructor.Invoke(new object[] { playable, player });
				else 
					return (T)constructor.Invoke(new object[] { playable });
            }
			return null;
        }

		static public PlayableControl GetFor(IPlayable playable, Player player)
		{
			var control = GetFor<PlayableControl>(Specials, playable, player);
			if (control != null)
				return control;
            return new GenericPlayableControl(playable, player.Master);
		}

		static public PlayableSubControl GetSubFor(IPlayable playable, Player player)
        {
            var control = GetFor<PlayableSubControl>(SubSpecials, playable, player);
            if (control != null)
                return control;
            return new GenericPlayableSubControl(playable);
		}
	}
}