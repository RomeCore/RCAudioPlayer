using RCAudioPlayer.Core.Players;

namespace RCAudioPlayer.WPF.Playables.Sub
{
	[PlayableSubControl(typeof(Track))]
	public partial class TrackSubControl
	{
		public TrackSubControl(Track track) : base(track)
		{
			InitializeComponent();

			titleText.Text = track.Title;
			artistText.Text = track.Artist;
		}
	}
}