using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace RCAudioPlayer.Core.Data
{
	public static class AudioDictionary
	{
		static private Dictionary<string, ConstructorInfo> UriConstructors { get; }

		static AudioDictionary()
		{
			var audioDataType = typeof(IAudioData);
			var constructorArguments = new Type[] { typeof(string) };
			UriConstructors = (from type in PluginManager.Types
							   where audioDataType.IsAssignableFrom(type)
							   let attribute = type.GetCustomAttribute<UriSupportsAttribute>()
							   where attribute != null
							   let constructor = type.GetConstructor(constructorArguments)
							   where constructor != null
							   from uriExtension in attribute.Extensions
							   select (uriExtension, constructor))
							   .ToDictionary(k => k.uriExtension, s => s.constructor);
		}

		public static IAudioData Get(string uri)
		{
			var extension = Path.GetExtension(uri).Substring(1);
			if (!string.IsNullOrEmpty(extension) && UriConstructors.TryGetValue(extension, out var _constructor))
				return (IAudioData)_constructor.Invoke(new object[] { uri });
			return new MediaFoundationAudioData(uri);
		}
	}
}