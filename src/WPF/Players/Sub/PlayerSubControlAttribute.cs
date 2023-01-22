using System;

namespace RCAudioPlayer.WPF.Players.Sub
{
	public class PlayerSubControlAttribute : Attribute
	{
		public Type PlayerType { get; }

		public PlayerSubControlAttribute(Type playerType)
		{
			PlayerType = playerType;
		}
	}
}