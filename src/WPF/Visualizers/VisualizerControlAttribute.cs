using System;

namespace RCAudioPlayer.WPF.Visualizers
{
	public class VisualizerControlAttribute : Attribute
	{
		public string Name { get; }

		public VisualizerControlAttribute(string name)
		{
			Name = name;
		}
	}
}