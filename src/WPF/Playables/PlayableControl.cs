﻿using System.Windows.Controls;
using RCAudioPlayer.Core.Playables;

namespace RCAudioPlayer.WPF.Playables
{
	public abstract class PlayableControl : UserControl
	{
		public IPlayable Playable { get; }

		public PlayableControl(IPlayable playable)
		{
			Playable = playable;
		}
	}
}