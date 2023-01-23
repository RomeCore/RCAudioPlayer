using RCAudioPlayer.Core.Playables;
using System;

namespace RCAudioPlayer.WPF.Playables
{
	public class PlayableControlAttribute : Attribute
	{
		public Type PlayableType { get; }

		public PlayableControlAttribute(Type playableType)
		{
			if (!typeof(IPlayable).IsAssignableFrom(playableType))
				throw new Exception("This type can't be assigned to Playable type");
			PlayableType = playableType;
		}
	}
}