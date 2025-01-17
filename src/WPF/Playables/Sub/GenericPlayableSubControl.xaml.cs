﻿using RCAudioPlayer.Core.Playables;

namespace RCAudioPlayer.WPF.Playables.Sub
{
	public partial class GenericPlayableSubControl
	{
		public GenericPlayableSubControl(IPlayable playable) : base(playable)
		{
			InitializeComponent();

			fullTitleText.Text = playable.FullTitle;
		}
	}
}