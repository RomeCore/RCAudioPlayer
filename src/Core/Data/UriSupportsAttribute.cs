using System;
using System.Linq;
using System.Collections.Generic;

namespace RCAudioPlayer.Core.Data
{
	// Decribes extensions that audio data can support
	public class UriSupportsAttribute : Attribute
	{
		public IReadOnlyList<string> Extensions { get; }

		public UriSupportsAttribute(params string[] extensions)
		{
			Extensions = extensions.ToList().AsReadOnly();
		}
	}
}