using System;
using System.Windows;
using System.Windows.Media;

namespace RCAudioPlayer.WPF.Visualizers
{
    [VisualizerControl("Spectrum")]
	public class SpectrumVisualizer : VisualizerControl
	{
		public int TargetBars { get; set; }

		public SpectrumVisualizer()
		{
			TargetBars = 250;
		}

		protected override void OnRender(DrawingContext dc)
		{
			if (samples.Count > 0 && waveFormat != null)
			{
				var fftData = VisualizerHelper.FFT(samples, waveFormat.Channels);
				var dataLength = fftData.GetLength(1) / 3;

				if (dataLength > TargetBars)
					fftData = VisualizerHelper.CountCompress(fftData, TargetBars, dataLength);
				else
					fftData = VisualizerHelper.Take(fftData, dataLength);

				int channels = fftData.GetLength(0);
				int bars = fftData.GetLength(1);

				double width = ActualWidth;
				double height = ActualHeight;
				double size = width / bars;

				var brush = new SolidColorBrush(App.PrimaryThemeColor);
				for (int i = 0; i < bars; i++)
				{
					var top = Math.Min(fftData[0, i], 1);
					var bottom = top;
					if (channels == 2)
						bottom = Math.Min(fftData[1, i], 1);

					dc.DrawRectangle(brush, null,
						new Rect(i * size, (1 - top) * height / 2,
						size, (bottom + top) * height / 2));
				}
			}
		}

		protected override void OnUpdate()
		{
			InvalidateVisual();
		}
	}
}