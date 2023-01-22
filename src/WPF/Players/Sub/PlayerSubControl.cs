using System.Windows.Controls;
using RCAudioPlayer.Core.Players;

namespace RCAudioPlayer.WPF.Players.Sub
{
	public class PlayerSubControl : UserControl
	{
		public Player Player { get; }

		public PlayerSubControl(Player player)
		{
			Player = player;
		}

		public virtual void OnClose()
        {
        }
	}
}