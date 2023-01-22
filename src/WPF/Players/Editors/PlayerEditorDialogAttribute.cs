using System;

namespace RCAudioPlayer.WPF.Players.Editors
{
	public class PlayerEditorDialogAttribute : Attribute
	{
		public Type PlayerType { get; }

		public PlayerEditorDialogAttribute(Type playerType)
		{
			PlayerType = playerType;
		}
	}
}