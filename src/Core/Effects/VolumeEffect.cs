using NAudio.Wave;
using System;
using RCAudioPlayer.Core.Effects.Attributes;

namespace RCAudioPlayer.Core.Effects
{
	[Effect("volume", "1,0")]
	public class VolumeEffect : AudioEffect
	{
		[Range("volume_factor", 0, 2, 0.01, order: 0)]
		public float Volume { get; set; }

		public override void Update()
		{
			base.Update();
		}

		public override void Process(Span<float> samples, WaveFormat format)
		{
			for (int i = 0; i < samples.Length; i++)
				samples[i] *= Volume;
		}

		public override string SerializePreset()
		{
			return Volume.ToString();
		}

		public override void DeserializePreset(string serialized)
		{
			Volume = float.Parse(serialized.Replace('.', ','));
		}
	}
}
