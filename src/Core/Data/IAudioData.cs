using System.IO;
using System.Collections.Generic;
using RCAudioPlayer.Core.Streams;

namespace RCAudioPlayer.Core.Data
{
    public interface IAudioData
	{
		public float Length { get; }
		public Dictionary<string, object> Data { get; }
		public AudioInput GetInput();
		public Stream GetRawInput();
	}
}