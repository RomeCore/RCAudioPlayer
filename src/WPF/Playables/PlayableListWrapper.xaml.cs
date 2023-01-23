using System.Windows.Controls;
using MahApps.Metro.IconPacks;
using MaterialDesignThemes.Wpf;
using RCAudioPlayer.Core.Players;
using RCAudioPlayer.WPF.Dialogs;

namespace RCAudioPlayer.WPF.Playables
{
	public partial class PlayableWrapper : UserControl
	{
		public PlayableControl PlayableControl { get; }
		public Player Player { get; }

		public PlayableWrapper(PlayableControl playableControl, Player player)
		{
			InitializeComponent();
			PlayableControl = playableControl;
			Player = player;

			playableHolder.Content = PlayableControl;
			ButtonProgressAssist.SetIsIndeterminate(playButton, true);
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
				MessageDialog.Show(exc);
			}
		}
    }
}
