using NAudio.Wave;
using System;
using NWaves.Filters;
using NWaves.Filters.BiQuad;
using RCAudioPlayer.Core.Effects.Attributes;

namespace RCAudioPlayer.Core.Effects
{
	[Effect("reverb", "0,4 0,06 4")]
	public class ReverbEffect : AudioEffect
	{
		private CombFeedbackFilter[,] _combFilters;
		private AllPassFilter[,] _allPassFilters;

		public const double RoomsizeScale = 0.15;
		public const double DampScale = 0.48;
		public const double DampOffset = 0.5;
		public const double Mix = 0.618;
		public const int MaxCombFilters = 32;
		public const int MaxAllPassFilters = 8;

		[Range("reverb_damp", 0, 1, 0.01, order: 1)]
		public double Damp { get; set; }
		[Range("reverb_decay", 0, 0.1, 0.005, order: 2)]
		public double Decay { get; set; }

		//[Range("reverb_allpass_count", 2, 8, 1, order: 3)]
		public int AllPassFiltersCount { get; set; } = 3;
		[Range("reverb_comb_count", 4, 16, 1, order: 4)]
		public int CombFiltersCount { get; set; } = 4;

		public ReverbEffect()
		{
			_allPassFilters = new AllPassFilter[MaxChannels, MaxAllPassFilters];
            _combFilters = new CombFeedbackFilter[MaxChannels, MaxCombFilters];

            for (int i = 0; i < MaxAllPassFilters; i++)
				for (int c = 0; c < MaxChannels; c++)
					_allPassFilters[c, i] = new AllPassFilter(100.0 / Math.Pow(3, i), Mix);

			for (int i = 0; i < MaxCombFilters; i++)
				for (int c = 0; c < MaxChannels; c++)
					_combFilters[c, i] = new CombFeedbackFilter(1600 + i * 350, 0, 0);
		}

		public override void Update()
		{
			double totalRoomsize = RoomsizeScale;
			double totaldamp = DampScale * Damp;

			double sampleRate = Format?.SampleRate ?? 44100;
            for (int i = 0; i < AllPassFiltersCount; i++)
                for (int c = 0; c < (Format?.Channels ?? MaxChannels); c++)
                    _allPassFilters[c, i].Change(100.0 / Math.Pow(3, i) / sampleRate, Mix);

			for (int i = 0; i < CombFiltersCount; i++)
			{
				var factor = DampOffset + totaldamp * (0.8 - Decay * i);
                for (int c = 0; c < (Format?.Channels ?? MaxChannels); c++)
                    _combFilters[c, i].Change(totalRoomsize, factor);
            }

			base.Update();
		}

		public override void Process(Span<float> samples, WaveFormat format)
		{
			for (int i = 0; i < samples.Length; i++)
			{
				var resultSample = samples[i];

				for (int a = 0; a < AllPassFiltersCount; a++)
					resultSample = _allPassFilters[i % format.Channels, a].Process(samples[i]);
				var apSample = resultSample;
				resultSample = 0;

				for (int c = 0; c < CombFiltersCount; c++)
					resultSample += _combFilters[i % format.Channels, c].Process(apSample);

				samples[i] = resultSample;
			}
		}

		public override string SerializePreset()
		{
			return $"{Damp} {Decay} {CombFiltersCount}";
		}

		public override void DeserializePreset(string serialized)
		{
			var splitted = serialized.Replace('.', ',').Split(' ', '\n', '\t');
			Damp = double.Parse(splitted[0]);
			Decay = double.Parse(splitted[1]);
			CombFiltersCount = int.Parse(splitted[2]);
		}
	}
}