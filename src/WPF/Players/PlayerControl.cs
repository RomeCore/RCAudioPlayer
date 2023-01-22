using System;
using System.Windows.Controls;
using RCAudioPlayer.Core.Players;

namespace RCAudioPlayer.WPF.Players
{
	public abstract class PlayerControl : UserControl
	{
		public Player Player { get; }

		public Action BackAction { get; set; } = () => { };

		public PlayerControl(Player player)
        {
			Player = player;
        }
	}
}