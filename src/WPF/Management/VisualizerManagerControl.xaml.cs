using System;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using NAudio.Wave;
using RCAudioPlayer.Core;
using RCAudioPlayer.Core.Streams;
using RCAudioPlayer.WPF.Files;
using RCAudioPlayer.WPF.Visualizers;

namespace RCAudioPlayer.WPF.Management
{
    public partial class VisualizerManagerControl
	{
		public PlayerMaster? Master { get; private set; }
		public string? VisualizerName { get; private set; }
		public VisualizerControl? Current { get; private set; }

		public int Steps { get => (int)stepSlider.Value; set => stepsLabel.Content = stepSlider.Value = value; }

		public VisualizerManagerControl()
		{
			InitializeComponent();

			visualizerSelector.ItemsSource = VisualizerControlDictionary.Visualizers;
			visualizerSelector.SelectionChanged += VisualizerSelector_SelectionChanged;

			var managerPanelSize = State.Get("visualizer_size", -1.0);
			if (managerPanelSize != -1.0)
				topRow.Height = new System.Windows.GridLength(managerPanelSize);

			App.Current.MainWindow.Closing += (s, e) =>
			{
				Deselect();
				State.Set("visualizer_size", topRow.Height.Value);
			};
		}

		private void VisualizerSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Select((string)visualizerSelector.SelectedValue);
		}

		public void Load(PlayerMaster master)
		{
			Load();

			Master = master;

			Steps = Files.State.Get("visualizer_steps", 12);
			stepSlider.ValueChanged += (s, e) => Files.State.Set("visualizer_steps", Steps = (int)e.NewValue);

			var startVisualizer = Files.State.Get("visualizer", string.Empty);
			if (!string.IsNullOrEmpty(startVisualizer) && VisualizerControlDictionary.Visualizers.Contains(startVisualizer))
				Select(startVisualizer);
		}

		private void PipelineStream_SamplesReceived(object? sender, PipelineStream.SamplesReceivedArgs e)
		{
			Visualize(e.Samples, e.OutputFormat, DateTime.Now + e.ProcessingTime);
		}

		private WaveFormat? _currentFormat;
		private ArraySegment<float> _currentSamples;
		
		public bool Select(string name)
		{
			if (Master == null)
				throw new InvalidOperationException("Visualizer manager is not loaded!");

            Files.State.Set("visualizer", name);
            Deselect();

			visualizerSelector.SelectedValue = VisualizerName = name;
			if (!string.IsNullOrEmpty(name))
				Current = VisualizerControlDictionary.Get(name);

			visualizerHolder.Content = Current;
			if (Current != null)
			{
				if (_currentSamples.Count > 0 && _currentFormat != null)
					Current.Visualize(_currentSamples, _currentFormat);
				if (Master != null)
					Master.PipelineStream.SamplesReceived += PipelineStream_SamplesReceived;
				return true;
            }
			return false;
		}

		public void Deselect()
		{
			if (Current != null)
			{
				Current.OnClose();
				if (Master != null)
					Master.PipelineStream.SamplesReceived -= PipelineStream_SamplesReceived;
			}
			Current = null;
		}

		private void Visualize(float[] samples, WaveFormat format, DateTime timeStart)
		{
			var visualizer = Current;
			var steps = Steps;

			if (visualizer != null && samples.Length > 0 && Master != null)
			{
				var thread = new Thread(() =>
				{
					int length = samples.Length / steps;
					var interval = SampleConvert.TimeSamples(length, format);

					for (int offset = 0; offset + length <= samples.Length; offset += length)
					{
						_currentSamples = new ArraySegment<float>(samples, offset, length);
						_currentFormat = format;
						visualizer.Visualize(_currentSamples, _currentFormat);

						if (offset + length * 2 <= samples.Length)
						{
							var timeSleep = interval - (DateTime.Now - timeStart);
							if (timeSleep.TotalMilliseconds > 0)
								Thread.Sleep(timeSleep);
							while (Master.Output.PlaybackState != PlaybackState.Playing)
								Thread.Sleep(10);
							timeStart = DateTime.Now;
						}
					}
				});
				thread.Start();
			}
		}
	}
}