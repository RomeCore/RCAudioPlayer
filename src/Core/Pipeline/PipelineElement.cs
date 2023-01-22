using System;
using NAudio.Wave;
using Newtonsoft.Json.Linq;

namespace RCAudioPlayer.Core.Pipeline
{
	public abstract class PipelineElement
	{
		public const int BufferStep = 4096;

		private bool _enabled;

		public virtual bool Enabled { get => _enabled; set
			{ 
				_enabled = value;
				if (!_enabled)
					ClearState();
			}
		}
		public virtual WaveFormat CurrentFormat { get; set; } = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);
		public abstract int AvailableSamples { get; }

		public abstract void Reset();
		public virtual void UpdateFormat(WaveFormat format)
        {
			CurrentFormat = format;
		}
		public virtual void ClearState()
		{
		}
		public virtual void Flush()
		{
		}

		protected abstract void Put(Span<float> samples);
		protected abstract Span<float> Get(int count);
		protected Span<float> Get() => Get(AvailableSamples);

		public abstract void Deserialize(JContainer? obj);
		public abstract JContainer Serialize();

		public PipelineElement? InputElement { get; set; }
		public virtual Span<float> Receive(int count)
		{
			if (InputElement == null)
				throw new ArgumentNullException(nameof(InputElement));

			while (AvailableSamples < count)
            {
				var samples = InputElement.Receive(BufferStep);
				if (samples.Length > 0)
                {
                    Put(samples);
					if (samples.Length < BufferStep)
                        break;
                }
				else
                {
                    Flush();
                    return Span<float>.Empty;
                }
			}

			var sampleCount = Math.Min(count, AvailableSamples);
			if (sampleCount > 0)
				return Get(sampleCount);
			else
				return Span<float>.Empty;
		}
	}
}