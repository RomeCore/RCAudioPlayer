using System.IO;
using System.Collections.Generic;
using RCAudioPlayer.Core.Streams;

namespace RCAudioPlayer.Core.Data
{
	// Describes some audio data (audio file, stream, playlist or something else)
	public interface IAudioData
	{
		// length in seconds
		public float Length { get; }
		// Contains data (like title, artists, uri)
		public Dictionary<string, object> Data { get; }
		// Used to get stream that contains samples and wave format to play it to output device
		public AudioInput GetInput();
		// Used to get raw stream (maybe file stream)
		public Stream GetRawInput();
	}
}