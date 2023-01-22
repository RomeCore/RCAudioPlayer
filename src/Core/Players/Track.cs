using System.Threading.Tasks;
using RCAudioPlayer.Core.Streams;
using RCAudioPlayer.Core.Effects;

namespace RCAudioPlayer.Core.Players
{
    public class Track : IPlayable
	{
		private TrackFile _source;

		public string Filename { get; }
		public string FullTitle { get; }
		public string Title { get; }
		public string Artist { get; }
		public float Duration { get; }

		public Track(string str)
		{
			Filename = str;
			_source = new TrackFile(Filename);

			Title = _source.Title;
			Artist = _source.Artist;
			Duration = _source.Length;

			if (string.IsNullOrWhiteSpace(Artist))
				FullTitle = Title;
			else
				FullTitle = Artist + " - " + Title;
		}

		public string Save()
		{
			return Filename;
		}

		public bool SkipForward()
		{
			return true;
		}

		public bool SkipBackward()
		{
			return true;
		}

		public async Task<AudioInput> GetInput()
		{
			return await Task.Run(() => _source.GetInput());
		}
	}
}