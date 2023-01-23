using RCAudioPlayer.Core.Playables;
using System.Threading.Tasks;

namespace RCAudioPlayer.Core.Players
{
    [Player("track", true)]
	public class TrackPlayer : ElementPlayer<Track>
	{
		public TrackPlayer(PlayerMaster master, string name) : base(master, name)
		{
		}

		public override Task<IPlayable> Create(string str)
		{
			return Task.Run(() => (IPlayable)new Track(str));
		}
	}
}