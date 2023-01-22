using System;
using NAudio.Wave;
using NWaves.Filters.BiQuad;
using RCAudioPlayer.Core.Effects.Attributes;

namespace RCAudioPlayer.Core.Effects
{
	[Effect("high_pass", "12000")]
	class HighPassEffect : AudioEffect
	{
		private HighPassFilter[] _filters;

		[Range("highpass_frequency", 100, 18000, 10, order: 0)]
		public int Frequency { get; set; }

		public HighPassEffect()
		{
			_filters = new HighPassFilter[MaxChannels];
			for (int i = 0; i < MaxChannels; i++)
				_filters[i] = new HighPassFilter(Frequency / 44100.0);
		}

		public override void Update()
		{
			for (int c = 0; c < (Format?.Channels ?? MaxChannels); c++)
				_filters[c].Change(Frequency / 44100.0);
			base.Update();
		}

		public override void Process(Span<float> samples, WaveFormat format)
		{
			for (int i = 0; i < samples.Length; i++)
				samples[i] = _filters[i % format.Channels].Process(samples[i]);
		}

		public override string SerializePreset()
		{
			return $"{Frequency}";
		}

		public override void DeserializePreset(string serialized)
		{
			Frequency = int.Parse(serialized);
		}
	}
}