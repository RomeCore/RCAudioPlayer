using System;
using NAudio.Wave;
using RCAudioPlayer.Core.Effects.Attributes;

namespace RCAudioPlayer.Core.Effects
{
	[Effect("echo", "0,5 0,25")]
	public class EchoEffect : AudioEffect
	{
		private NWaves.Effects.EchoEffect[] _channelEffects;

		[Range("echo_delay", 0.01, 5, 0.01, order: 0)]
		public double Delay { get; set; }
		[Range("echo_feedback", 0.01, 0.99, 0.01, order: 1)]
		public double Feedback { get; set; }

		public EchoEffect()
		{
			_channelEffects = new NWaves.Effects.EchoEffect[MaxChannels];
			for (int c = 0; c < MaxChannels; c++)
				_channelEffects[c] = new NWaves.Effects.EchoEffect(44100, 0, 0);
		}

		public override void Update()
		{
			for (int c = 0; c < MaxChannels; c++)
			{
				_channelEffects[c].Delay = (float)Delay;
				_channelEffects[c].Feedback = (float)Feedback;
			}
			base.Update();
		}

		public override void Process(Span<float> samples, WaveFormat format)
		{
			for (int i = 0; i < samples.Length; i++)
				samples[i] = _channelEffects[i % format.Channels].Process(samples[i]);
		}

		public override void DeserializePreset(string serialized)
		{
			var splitted = serialized.Replace('.', ',').Split(' ', '\n', '\t');
			Delay = double.Parse(splitted[0]);
			Feedback = double.Parse(splitted[1]);
		}

		public override string SerializePreset()
		{
			return $"{Delay} {Feedback}";
		}
	}
}