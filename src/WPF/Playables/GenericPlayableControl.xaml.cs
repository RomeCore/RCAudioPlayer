using MahApps.Metro.IconPacks;
using MaterialDesignThemes.Wpf;
using RCAudioPlayer.Core;
using RCAudioPlayer.Core.Players;

namespace RCAudioPlayer.WPF.Playables
{
	public partial class GenericPlayableControl
	{
		public PlayerMaster Master { get; }

		public GenericPlayableControl(IPlayable playable, PlayerMaster master) : base(playable)
		{
			InitializeComponent();
			Master = master;

			titleText.Text = playable.FullTitle;
			ButtonProgressAssist.SetIsIndeterminate(playButton, true);
		}

        private async void PlayButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var materialIcon = (PackIconMaterial)playButton.Content;
			try
			{
				ButtonProgressAssist.SetIsIndicatorVisible(playButton, true);
				Master.Load(await Playable.GetInput());
				materialIcon.Kind = PackIconMaterialKind.Play;
				ButtonProgressAssist.SetIsIndicatorVisible(playButton, false);
			}
			catch
			{
				materialIcon.Kind = PackIconMaterialKind.AlertCircleOutline;
				ButtonProgressAssist.SetIsIndicatorVisible(playButton, false);
			}
		}
    }
}