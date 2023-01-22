using System;

namespace RCAudioPlayer.WPF.Players
{
	public class PlayerControlAttribute : Attribute
	{
		public Type PlayerType { get; }

		public PlayerControlAttribute(Type playerType)
		{
			if (!typeof(Core.Players.Player).IsAssignableFrom(playerType))
				throw new Exception("This type can't be assigned to Player type");
			PlayerType = playerType;
		}
	}
}