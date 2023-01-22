using System;

namespace RCAudioPlayer.Core.Players
{
	public class PlayerAttribute : Attribute
	{
		public string TypeName { get; }
		public bool CanEdit { get; }

		public PlayerAttribute(string className, bool canEdit = false)
		{
			TypeName = className;
			CanEdit = canEdit;
		}
	}
}