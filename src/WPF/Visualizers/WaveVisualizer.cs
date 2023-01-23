using System;
using System.Windows;
using System.Windows.Media;

namespace RCAudioPlayer.WPF.Visualizers
{
	[VisualizerControl("Wave")]
	public class WaveVisualizer : VisualizerControl
	{
		public int MaxBars { get; set; }

		public WaveVisualizer()
		{
			MaxBars = 500;
		}

		protected override void OnRender(DrawingContext dc)
		{
			if (samples.Count > 0 && waveFormat != null)
			{
				var sampleData = VisualizerHelper.Split(samples, waveFormat.Channels);
				sampleData = VisualizerHelper.CountCompress(sampleData, MaxBars);

				int channels = sampleData.GetLength(0);
				int bars = sampleData.GetLength(1);

				double width = ActualWidth;
				double height = ActualHeight;
				double size = width / bars;

				var primaryColor = App.PrimaryThemeColor;
				var primaryTransparentColor = App.PrimaryThemeColor;
				primaryTransparentColor.A = 0;

				var topBrush = new LinearGradientBrush();
				topBrush.GradientStops.Add(new GradientStop(primaryColor, 0));
				topBrush.GradientStops.Add(new GradientStop(primaryTransparentColor, 1));
				topBrush.StartPoint = new Point(0, 1);
				topBrush.EndPoint = new Point(0, 0);

				var bottomBrush = new LinearGradientBrush();
				bottomBrush.GradientStops.Add(new GradientStop(primaryColor, 0));
				bottomBrush.GradientStops.Add(new GradientStop(primaryTransparentColor, 1));
				bottomBrush.StartPoint = new Point(0, 0);
				bottomBrush.EndPoint = new Point(0, 1);

				for (int b = 0; b < bars; b++)
				{
					double length = 0;
					for (int c = 0; c < channels; c++)
						length += sampleData[c, b];
					length = Math.Clamp(length / (channels * 10), -1, 1);
					double absoluteLength = Math.Abs(length);
					
					if (length > 0)
						dc.DrawRectangle(topBrush, null,
							new Rect(b * size, height / 2,
							size, absoluteLength * height / 2));
					else if (length < 0)
						dc.DrawRectangle(bottomBrush, null,
							new Rect(b * size, (1 - absoluteLength) * height / 2,
							size, absoluteLength * height / 2));
				}
			}
		}

		protected override void OnUpdate()
		{
			InvalidateVisual();
		}
	}
}