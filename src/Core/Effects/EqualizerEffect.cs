using System;
using NAudio.Wave;
using NAudio.Dsp;
using RCAudioPlayer.Core.Effects.Attributes;

namespace RCAudioPlayer.Core.Effects
{
	[Effect("equalizer", "0 0 0 0 0 0 0 0")]
	public class EqualizerEffect : AudioEffect
	{
		public class Band
		{
			public float Frequency { get; }
			public float Bandwidth { get; }
			public float Gain { get; set; }

			public Band(float frequency, float bandwidth, float gain = 0)
			{
				Frequency = frequency;
				Bandwidth = bandwidth;
				Gain = gain;
			}
		}

		public const int MaxBands = 32;
		public readonly int bandsCount = 8;

		private readonly BiQuadFilter[,] filters;

		public Band[] Bands { get; }

		public EqualizerEffect()
		{
			Bands = new Band[bandsCount];
			for (int i = 0; i < bandsCount / 2; i++)
				Bands[i] = new Band (100 * (int)Math.Pow(2, i), 0.8f, 0);
			for (int i = 0; i < bandsCount / 2; i++)
				Bands[i + bandsCount / 2] = new Band (1200 * (int)Math.Pow(2, i), 0.8f, 0);

			filters = new BiQuadFilter[MaxChannels, MaxBands];
			for (int c = 0; c < MaxChannels; c++)
				for (int b = 0; b < Bands.Length; b++)
					filters[c, b] = BiQuadFilter.PeakingEQ(DefaultSampleRate, 0, 0, 0);
		}

		public override void Update()
		{
			for (int c = 0; c < (Format?.Channels ?? MaxChannels); c++)
				for (int b = 0; b < Bands.Length; b++)
					filters[c, b].SetPeakingEq(Format?.SampleRate ?? 44100, Bands[b].Frequency, Bands[b].Bandwidth, Bands[b].Gain);
			base.Update();
		}

		public override string SerializePreset()
		{
			string result = string.Empty;
			foreach (var band in Bands)
				result += band.Gain.ToString("0.0") + " ";
			return result.Substring(0, result.Length - 1);
		}

		public override void DeserializePreset(string serialized)
		{
			var gains = serialized.Split(' ', '\n', '\t');
			for (int i = 0; i < Bands.Length; i++)
			{
				float value = 0.0f;
				if (i < gains.Length)
					value = float.Parse(gains[i].Replace('.', ','));
				Bands[i].Gain = value;
			}
		}

		public override void Process(Span<float> samples, WaveFormat format)
		{
			for (int i = 0; i < samples.Length; i++)
				for (int b = 0; b < Bands.Length; b++)
					samples[i] = filters[i % format.Channels, b].Transform(samples[i]);
		}
	}
}