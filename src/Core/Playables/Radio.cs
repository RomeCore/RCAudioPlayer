using NAudio.Wave;
using RCAudioPlayer.Core.Data;
using RCAudioPlayer.Core.Streams;
using System.Threading.Tasks;

namespace RCAudioPlayer.Core.Playables
{
	public class Radio : IPlayable
	{
		private IAudioData _source;

		public string BaseUrl { get; private set; }
		public string FullTitle { get; private set; }
		public string StreamUrl { get; private set; }

		public Radio(string str)
		{
			BaseUrl = str;
			if ((_source = AudioDictionary.Get(BaseUrl)) is IPlaylist data)
			{
				var track = data.List[0];
				FullTitle = track.Data.ContainsKey("artist") ? track.Data["artist"] + " - " + track.Data["title"]
					: track.Data.ContainsKey("title") ? (string)track.Data["title"] : "";
				StreamUrl = track.Data["uri"] as string ?? "";
			}
			else
			{
				throw new System.Exception("This format is not supported!");
			}
		}

		public string Save()
		{
			return BaseUrl;
		}

		public bool SkipBackward()
		{
			return true;
		}

		public bool SkipForward()
		{
			return true;
		}

		public async Task<AudioInput> GetInput()
		{
			return await Task.Run(() => AudioInput.FromStream(new MediaFoundationReader(StreamUrl), _source));
		}
	}
}