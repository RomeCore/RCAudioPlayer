using System;
using RCAudioPlayer.Core;

namespace RCAudioPlayer.WPF.Pipeline
{
	[PipelineElementControl(typeof(Core.Pipeline.SoundTouch))]
	public partial class SoundTouchControl
	{
		public Core.Pipeline.SoundTouch SoundTouch { get; }

		public double Tempo { get => tempoSlider.Value; set 
			{
				value = Math.Clamp(value, 0.5, 2.0);
				SoundTouch.Tempo = value;
				tempoSlider.Value = value;
				tempoLabel.Content = value.ToStr();
			}
		}
		
		public double Pitch { get => pitchSlider.Value; set 
			{
				value = Math.Clamp(value, 0.5, 2.0);
				if (SoundTouch != null)
					SoundTouch.Pitch = value;
				pitchSlider.Value = value;
				pitchLabel.Content = value.ToStr();
			}
		}
		
		public double Rate { get => rateSlider.Value; set 
			{
				value = Math.Clamp(value, 0.5, 2.0);
				if (SoundTouch != null)
					SoundTouch.Rate = value;
				rateSlider.Value = value;
				rateLabel.Content = value.ToStr();
			}
		}

		public SoundTouchControl(Core.Pipeline.SoundTouch soundTouch) : base(soundTouch)
		{
			InitializeComponent();
			SoundTouch = soundTouch;

			Update();

			tempoSlider.ValueChanged += (s, e) => Tempo = e.NewValue;
			pitchSlider.ValueChanged += (s, e) => Pitch = e.NewValue;
			rateSlider.ValueChanged += (s, e) => Rate = e.NewValue;
		}

		public void Update()
		{
			if (SoundTouch != null)
			{
				Tempo = SoundTouch.Tempo;
				Pitch = SoundTouch.Pitch;
				Rate = SoundTouch.Rate;
			}
		}

		public override void Reset()
		{
			base.Reset();
			Update();
		}
	}
}