using MahApps.Metro.IconPacks;
using RCAudioPlayer.Core.Players;
using RCAudioPlayer.WPF.Playables.Sub;

namespace RCAudioPlayer.WPF.Playables.Main
{
    [PlayableControl(typeof(Track))]
	public partial class TrackControl : PlayableControl
	{
        public Track Track { get; }
		public Player Player { get; }

		public TrackControl(Track track, Player player) : base(track)
		{
			InitializeComponent();

			Track = track;
			Player = player;

			titleText.Text = track.Title;
			artistText.Text = track.Artist;
		}
	}
}