using System.Windows.Controls;
using RCAudioPlayer.Core;
using RCAudioPlayer.Core.Streams;

namespace RCAudioPlayer.WPF.Players.Sub
{
	public partial class PlaybackPanelSubControl : UserControl
	{
		public PlayerMaster Master { get; }
		public PipelineStream PipelineStream { get; }

		public PlaybackPanelSubControl(PlayerMaster master)
		{
			InitializeComponent();
			Master = master;
			PipelineStream = master.PipelineStream;

			lengthLabel.Content = "(0:00)";
			PipelineStream.PositionChanged += PipelineStream_PositionChanged;

			master.Output.Volume = Files.State.Get("volume", master.Output.Volume);
			volumeButton.Volume = master.Output.Volume * 100;
			MaterialDesignThemes.Wpf.ButtonProgressAssist.SetValue(volumeButton, master.Output.Volume * 100);
			volumeButton.VolumeChanged += VolumeButton_VolumeChanged;
		}

		private void VolumeButton_VolumeChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
		{
			Files.State.Set("volume", Master.Output.Volume = (float)e.NewValue / 100);
			MaterialDesignThemes.Wpf.ButtonProgressAssist.SetValue(volumeButton, e.NewValue);
		}

		public void OnClose()
		{
			PipelineStream.PositionChanged -= PipelineStream_PositionChanged;
		}

		private void PipelineStream_PositionChanged(object? sender, PipelineStream.PositionChangedArgs e)
		{
			if (e.NewLength == 0)
				lengthLabel.Content = $"({SampleConvert.TimeBytes(e.NewPosition, PipelineStream.InputWaveFormat).ToStr()})";
			else
				lengthLabel.Content = $"({SampleConvert.TimeBytes(e.NewPosition, PipelineStream.InputWaveFormat).ToStr()}/{SampleConvert.TimeBytes(e.NewLength, PipelineStream.InputWaveFormat).ToStr()})";
		}
	}
}