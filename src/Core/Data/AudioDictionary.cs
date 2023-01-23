using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace RCAudioPlayer.Core.Data
{
	// Provides methods to get audio data by uri and something else... (TODO: expand functionality (get data by it header or ...))
	public static class AudioDictionary
	{
		static private Dictionary<string, ConstructorInfo> UriConstructors { get; }

		static AudioDictionary()
		{
			var uriAudioDataType = typeof(IUriAudioData);
			var uriConstructorArguments = new Type[] { typeof(string) };
			UriConstructors = (from type in PluginManager.Types
							   where uriAudioDataType.IsAssignableFrom(type)
							   let attribute = type.GetCustomAttribute<UriSupportsAttribute>()
							   where attribute != null
							   let constructor = type.GetConstructor(uriConstructorArguments)
							   where constructor != null
							   from uriExtension in attribute.Extensions
							   select (uriExtension, constructor))
							   .ToDictionary(k => k.uriExtension, s => s.constructor);
		}

		// Getting audio data by uri's extension
		public static IUriAudioData Get(string uri)
		{
			var extension = Path.GetExtension(uri).Substring(1);
			if (!string.IsNullOrEmpty(extension) && UriConstructors.TryGetValue(extension, out var _constructor))
				return (IUriAudioData)_constructor.Invoke(new object[] { uri });
			return new MediaFoundationAudioData(uri);
		}
	}
}