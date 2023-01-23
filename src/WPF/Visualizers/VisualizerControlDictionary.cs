using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using RCAudioPlayer.Core;

namespace RCAudioPlayer.WPF.Visualizers
{
	public static class VisualizerControlDictionary
	{
		static private Dictionary<string, ConstructorInfo> Dictionary { get; }

		static public IEnumerable<string> Visualizers => Dictionary.Keys;

		static VisualizerControlDictionary()
		{
			var playerControlType = typeof(VisualizerControl);
			Dictionary = (from type in PluginManager.Types
						  where playerControlType.IsAssignableFrom(type)
						  let attribute = type.GetCustomAttribute<VisualizerControlAttribute>()
						  where attribute != null
						  let constructor = type.GetConstructor(Type.EmptyTypes)
						  where constructor != null
						  select (attribute.Name, constructor))
						 .ToDictionary(k => k.Item1, s => s.constructor);
		}

		static public VisualizerControl? Get(string name)
		{
			if (Dictionary.TryGetValue(name, out var constructor))
				return (VisualizerControl)constructor.Invoke(null);
			return null;
		}
	}
}