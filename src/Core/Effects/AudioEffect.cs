using System;
using NAudio.Wave;

namespace RCAudioPlayer.Core.Effects
{
	public abstract class AudioEffect
	{
		public class PresetChangedArgs
		{
			public string Name { get; private set; }
			public string Data { get; private set; }

			public PresetChangedArgs(string name, string data)
			{
				Name = name;
				Data = data;
			}
		}

		private bool _invokeUpdateEvent = true;

		public const int MaxChannels = 8;
		public const int DefaultSampleRate = AudioOutput.DefaultSampleRate;

		public bool Enabled { get; set; }
		public WaveFormat? Format { get; protected set; }
		public string CustomPreset { get; set; } = string.Empty;
		public string PresetName { get; set; } = string.Empty;

		public event EventHandler<PresetChangedArgs> PresetChanged = (s, e) => { };
		public event EventHandler OnUpdate = (s, e) => { };
		public event EventHandler Updated = (s, e) => { };
		public event EventHandler OnDelete = (s, e) => { };

		public abstract string SerializePreset();
		public abstract void DeserializePreset(string serialized);
		public abstract void Process(Span<float> samples, WaveFormat format);

		public AudioEffect()
		{
			CustomPreset = EffectDictionary.FindByType(GetType())?.Attribute.DefaultPreset ?? string.Empty;
		}

		~AudioEffect()
		{
			OnDelete(this, new EventArgs());
		}

		public virtual void Update()
		{
			OnUpdate(this, new EventArgs());
			if (_invokeUpdateEvent)
				Updated(this, new EventArgs());
			_invokeUpdateEvent = true;
		}

		public void LoadPreset(string name, string serialized)
		{
			PresetName = name;
			DeserializePreset(serialized);
			PresetChanged(this, new PresetChangedArgs(name, serialized));

			_invokeUpdateEvent = false;
			Update();
		}

		public void UpdateWith(WaveFormat format)
		{
			Format = format;
			_invokeUpdateEvent = false;
			Update();
		}
	}
}