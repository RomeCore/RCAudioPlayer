using System.Windows.Controls;
using MahApps.Metro.IconPacks;
using MaterialDesignThemes.Wpf;
using RCAudioPlayer.Core.Players;
using RCAudioPlayer.Core.Playables;
using RCAudioPlayer.WPF.Dialogs;

namespace RCAudioPlayer.WPF.Playables
{
	public partial class PlayableListWrapper : UserControl
	{
		public PlayableControl PlayableControl { get; }
		public Player Player { get; }

		public PlayableListWrapper(PlayableControl playableControl, Player player)
		{
			InitializeComponent();
			PlayableControl = playableControl;
			Player = player;

			if (playableControl.Playable is ErrorPlayable)
				playButton.Visibility = System.Windows.Visibility.Collapsed;
			else
				ButtonProgressAssist.SetIsIndeterminate(playButton, true);
			playableHolder.Content = PlayableControl;
		}

		private async void PlayButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var materialIcon = (PackIconMaterial)playButton.Content;
			try
			{
				if (Player is ElementPlayer elementPlayer)
				{
					ButtonProgressAssist.SetIsIndicatorVisible(playButton, true);
					await elementPlayer.Play(PlayableControl.Playable);
					materialIcon.Kind = PackIconMaterialKind.Play;
					ButtonProgressAssist.SetIsIndicatorVisible(playButton, false);
				}
				else
					throw new System.Exception();
			}
			catch (System.Exception exc)
			{
				materialIcon.Kind = PackIconMaterialKind.AlertCircleOutline;
				ButtonProgressAssist.SetIsIndicatorVisible(playButton, false);
				ExceptionDialog.Show(exc);
			}
		}
	}
}