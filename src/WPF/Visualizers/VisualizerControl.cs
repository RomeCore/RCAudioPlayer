using System;
using System.Windows.Controls;
using System.Windows.Threading;
using NAudio.Wave;

namespace RCAudioPlayer.WPF.Visualizers
{
	public abstract class VisualizerControl : UserControl
	{
		private bool _update = false;
		private DispatcherTimer _timer;

		protected ArraySegment<float> samples = ArraySegment<float>.Empty;
		protected WaveFormat? waveFormat;

		public VisualizerControl()
		{
			_timer = new DispatcherTimer(DispatcherPriority.Normal, Dispatcher);

			_timer.Interval = TimeSpan.FromMilliseconds(5);
			_timer.Tick += Timer_Tick;

			_timer.Start();
		}

		private void Timer_Tick(object? sender, EventArgs e)
		{
			if (_update)
			{
				OnUpdate();
				_update = false;
			}
		}

		public void OnClose()
		{
			_timer.Stop();
		}

		protected virtual void OnUpdate() { }

		public void Visualize(ArraySegment<float> samples, WaveFormat waveFormat)
		{
			this.samples = samples;
			this.waveFormat = waveFormat;
			_update = true;
		}
	}
}