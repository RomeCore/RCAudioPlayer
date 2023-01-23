using System;
using System.Collections.Generic;
using RCAudioPlayer.Core.Players;
using RCAudioPlayer.Core.Playables;

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

			if (track.ThumbnailData != null)
				thumbnail.Source = Utils.GetBitmapFromBytes(track.ThumbnailData);
			else
				thumbnail.Visibility = System.Windows.Visibility.Collapsed;

			ContextMenu = new Dictionary<string, Action>
			{
				{ "show_in_explorer", () => Core.Utils.ShowInExplorer(track.Filename) }
			}.GetContextMenu();
		}
	}
}