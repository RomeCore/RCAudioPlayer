using RCAudioPlayer.Core;
using RCAudioPlayer.WPF.Players;
using RCAudioPlayer.WPF.Players.Sub;

namespace RCAudioPlayer.WPF.Management
{
    public partial class PlaybackPanel
	{
		public PlaybackPanel()
		{
			InitializeComponent();
		}

		public void Load(PlayerManager playerManager)
		{
			Load();

			streamSlider.PipelineStream = playerManager.Master.PipelineStream;
			playerManager.OnChanged += PlayerManager_OnChanged;
		}

		private void PlayerManager_OnChanged(object? sender, PlayerManager.ChangedArgs e)
		{
			var player = e.Player;

			if (playerHolder.Content is PlayerSubControl playerSubControl)
				playerSubControl.OnClose();
			playerHolder.Content = PlayerControlDictionary.GetSubFor(player);
		}
	}
}