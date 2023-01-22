using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shell;
using RCAudioPlayer.Core;
using RCAudioPlayer.Core.Streams;

namespace RCAudioPlayer.WPF
{
    public class PipelineStreamSlider : Slider
	{
		private PipelineStream? _pipelineStream;
		private Track? _track;
		private bool _dragging = false;

		public PipelineStream? PipelineStream
		{
			get => _pipelineStream; set
			{
				_pipelineStream = value;
				if (_pipelineStream != null)
					_pipelineStream.PositionChanged += (s, e) =>
					{
						if (e.OldLength != e.NewLength)
						{
							Maximum = e.NewLength;
							if (e.NewLength == 0)
								MainWindow.Current.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate;
							else
								MainWindow.Current.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
						}
						if (e.NewLength != 0)
							MainWindow.Current.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
						MainWindow.Current.TaskbarItemInfo.ProgressValue = (e.NewPosition + 1) / (double)(Maximum + 1);

						if (!_dragging)
							Value = e.NewPosition;
					};
			}
		}

		public PipelineStreamSlider()
		{
			Style = (Style)App.Current.Resources[typeof(Slider)];

			var output = MainWindow.PlayerManager.Master.Output;
			output.Paused += (s, e) => MainWindow.Current.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Paused;
			output.Stopped += (s, e) => MainWindow.Current.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_track = Template.FindName("PART_Track", this) as Track ?? throw new System.Exception("Slider must contain track!");
		}

		private void CorrectTooltip(object source)
		{
			if (_pipelineStream != null && source is Thumb thumb && thumb.ToolTip is ToolTip toolTip)
			{
                var time = SampleConvert.TimeBytes((long)Value, _pipelineStream.InputWaveFormat);
				toolTip.Content = time.ToStr();
			}
		}

		protected override void OnThumbDragStarted(DragStartedEventArgs e)
		{
			base.OnThumbDragStarted(e);
			CorrectTooltip(e.OriginalSource);
			_dragging = true;
		}

		protected override void OnThumbDragDelta(DragDeltaEventArgs e)
		{
			base.OnThumbDragDelta(e);
			CorrectTooltip(e.OriginalSource);
		}

		protected override void OnThumbDragCompleted(DragCompletedEventArgs e)
		{
			base.OnThumbDragCompleted(e);

			if (e.OriginalSource is Thumb thumb)
				thumb.ToolTip = null;

			_dragging = false;
			if (_pipelineStream != null && _pipelineStream.Length > 0)
				_pipelineStream.Position = (long)Value;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (e.LeftButton == MouseButtonState.Pressed)
			{
				var thumb = _track?.Thumb;
				if (thumb != null)
					thumb.RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left)
					{
						RoutedEvent = MouseLeftButtonDownEvent,
						Source = e.Source
					});
			}
		}
	}
}