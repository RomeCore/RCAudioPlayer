using System.IO;
using System.Collections.Generic;
using NAudio.Wave;
using RCAudioPlayer.Core.Streams;

namespace RCAudioPlayer.Core.Data
{
	// Audio data that uses media foundation
	// Windows media foundation uses installed codecs on computer and gives us audio data
	public class MediaFoundationAudioData : IUriAudioData
	{
		public string Uri { get; }
		public float Length { get; }
		public Dictionary<string, object> Data { get; }

		private AudioInput _input;

		public MediaFoundationAudioData(string uri, Dictionary<string, object>? data = null)
		{
			Uri = uri;
			Data = data ?? new Dictionary<string, object>();

			var mfStream = new MediaFoundationReader(uri);
			_input = AudioInput.FromStream(mfStream, this);
		}

		public AudioInput GetInput() => _input;
		public Stream GetRawInput() => NetClient.GetStream(Uri);
	}
}