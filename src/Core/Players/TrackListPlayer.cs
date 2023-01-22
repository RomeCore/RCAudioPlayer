using System.Threading.Tasks;

namespace RCAudioPlayer.Core.Players
{
	[Player("track_list")]
	public class TrackListPlayer : ListPlayer<Track>
	{
		public TrackListPlayer(PlayerMaster master, string name) : base(master, name)
		{
		}

        public override Task<IPlayable> Create(string str)
        {
			return Task.Run(() => (IPlayable)new Track(str));
        }
	}
}