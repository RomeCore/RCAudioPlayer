using System;

namespace RCAudioPlayer.WPF.Playables
{
	public class PlayableControlAttribute : Attribute
	{
		public Type PlayableType { get; }

		public PlayableControlAttribute(Type playableType)
		{
			if (!typeof(Core.Players.IPlayable).IsAssignableFrom(playableType))
				throw new Exception("This type can't be assigned to Playable type");
			PlayableType = playableType;
		}
	}
}