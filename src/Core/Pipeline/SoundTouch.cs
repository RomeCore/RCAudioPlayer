using System;
using System.IO;
using System.Runtime.InteropServices;
using NAudio.Wave;
using RCAudioPlayer.Core;
using global::SoundTouch;
using Newtonsoft.Json.Linq;

namespace RCAudioPlayer.Core.Pipeline
{
	[Pipeline("sound_touch")]
	public class SoundTouch : PipelineElement
	{
		private readonly SoundTouchProcessor _processor;
		private int _channels = 2;

		public SoundTouch()
		{
			_processor = new SoundTouchProcessor();

			_processor.Tempo = 1.0;
			_processor.Pitch = 1.0;
			_processor.Rate = 1.0;
		}

		public double Tempo
		{
			get => _processor.Tempo;
			set
			{
				lock (SyncLock)
				{
					_processor.Tempo = value;
				}
			}
		}

		public double Pitch
		{
			get => _processor.Pitch;
			set
			{
				lock (SyncLock)
				{
					_processor.Pitch = value;
				}
			}
		}

		public double Rate
		{
			get => _processor.Rate;
			set
			{
				lock (SyncLock)
				{
					_processor.Rate = value;
				}
			}
		}

		public double TempoChange
		{
			get => _processor.TempoChange;
			set
			{
				lock (SyncLock)
				{
					_processor.TempoChange = value;
				}
			}
		}

		public double PitchOctaves
		{
			get => _processor.PitchOctaves;
			set
			{
				lock (SyncLock)
				{
					_processor.PitchOctaves = value;
				}
			}
		}

		public double PitchSemiTones
		{
			get => _processor.PitchSemiTones;
			set
			{
				lock (SyncLock)
				{
					_processor.PitchSemiTones = value;
				}
			}
		}

		public double RateChange
		{
			get => _processor.RateChange;
			set
			{
				lock (SyncLock)
				{
					_processor.RateChange = value;
				}
			}
		}

		internal bool IsFlushed { get; private set; }

		internal object SyncLock { get; } = new object();

		public void OptimizeForSpeech()
		{
			// Change settings to optimize for Speech.
			_processor.SetSetting(SettingId.SequenceDurationMs, 50);
			_processor.SetSetting(SettingId.SeekWindowDurationMs, 10);
			_processor.SetSetting(SettingId.OverlapDurationMs, 20);
			_processor.SetSetting(SettingId.UseQuickSeek, 0);
		}

		public void Clear()
		{
			lock (SyncLock)
			{
				_processor.Clear();
				IsFlushed = false;
			}
		}

		public override int AvailableSamples => _processor.AvailableSamples;

		public override void Reset()
		{
			Tempo = 1;
			Pitch = 1;
			Rate = 1;
		}

		public override void ClearState() => Clear();
		public override void Flush()
        {
			lock (SyncLock)
			{
				if (!IsFlushed)
                {
                    _processor.Flush();
                    IsFlushed = true;
                }
			}
        }

		public override void UpdateFormat(WaveFormat format)
		{
			_processor.SampleRate = format.SampleRate;
			_processor.Channels = _channels = format.Channels;
		}

		protected override void Put(Span<float> samples)
		{
			_processor.PutSamples(samples, samples.Length / _channels);
        }

		protected override Span<float> Get(int count)
		{
			var buffer = new float[count].AsSpan();
			var readSamples = _processor.ReceiveSamples(buffer, count / _channels);
			return buffer.Slice(0, readSamples * _channels);
		}

		public override void Deserialize(JContainer? obj)
		{
			if (obj != null)
			{
				Tempo = obj["tempo"]?.Value<double>() ?? throw new Exception();
				Pitch = obj["pitch"]?.Value<double>() ?? throw new Exception();
				Rate = obj["rate"]?.Value<double>() ?? throw new Exception();
			}
			else
				Reset();
		}

		public override JContainer Serialize()
		{
			var obj = new JObject();

			obj.Add("tempo", JToken.FromObject(Tempo));
			obj.Add("pitch", JToken.FromObject(Pitch));
			obj.Add("rate", JToken.FromObject(Rate));

			return obj;
		}
	}
}