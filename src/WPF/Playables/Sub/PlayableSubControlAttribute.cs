using System;

namespace RCAudioPlayer.WPF.Playables.Sub
{
	public class PlayableSubControlAttribute : Attribute
	{
		public Type PlayableType { get; }

		public PlayableSubControlAttribute(Type playableType)
		{
			PlayableType = playableType;
		}
	}
}