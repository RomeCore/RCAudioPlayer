using RCAudioPlayer.Core.Streams;
using System.Threading.Tasks;

namespace RCAudioPlayer.Core.Playables
{
	public interface IPlayable
	{
		public string FullTitle { get; }

		public string Save();

		// return true if skip to next track, false if stay on current track
		public bool SkipForward();
		public bool SkipBackward();

		public Task<AudioInput> GetInput();
	}
}