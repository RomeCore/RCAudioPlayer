using System;
using System.Linq;
using System.Collections.Generic;
using NAudio.Wave;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RCAudioPlayer.Core.Effects;

namespace RCAudioPlayer.Core.Pipeline
{
	[Pipeline("effect_chain")]
	public class EffectChain : PipelineElement
	{
		private struct EffectSerializeData
		{
			[JsonProperty("enable")]
			public bool enabled;
			[JsonProperty("id")]
			public string id;
			[JsonProperty("preset")]
			public string presetName;
			[JsonProperty("custom")]
			public string customPreset;
		}

		public class ListUpdatedArgs
		{
			public IReadOnlyList<AudioEffect> Effects { get; }

			public ListUpdatedArgs(IReadOnlyList<AudioEffect> effects)
			{
				Effects = effects;
			}
		}

		private readonly List<AudioEffect> _effects;
		private Queue<float> _samples;

		public IReadOnlyList<AudioEffect> Effects => _effects;
		public event EventHandler<ListUpdatedArgs> ListUpdated = (s, e) => { };

		private AudioEffect? MakeDefaultInstance(EffectDictionary.Data effectData)
		{
			var instance = EffectDictionary.CreateEffectInstance(effectData);
			if (instance != null)
				effectData.Presets.Select(instance, string.Empty, effectData.Attribute.DefaultPreset);
			return instance;
		}

		public EffectChain()
		{
			_effects = new List<AudioEffect>();
			_samples = new Queue<float>();
		}

		public AudioEffect? Add(string id)
		{
			return Add(EffectDictionary.FindById(id));
		}

		public AudioEffect? Add(Type type)
		{
			return Add(EffectDictionary.FindByType(type));
		}

		public AudioEffect? Add(EffectDictionary.Data? data)
		{
			return Insert(data, Effects.Count);
		}

		public AudioEffect? Insert(EffectDictionary.Data? data, int index)
		{
			if (data != null)
			{
				var instance = MakeDefaultInstance(data);
				return instance != null && Insert(instance, index) ? instance : null;
			}
			return null;
		}

		public bool Add(AudioEffect effect)
		{
			return Insert(effect, Effects.Count);
		}

		public bool Add(IEnumerable<AudioEffect> effects)
		{
			return Insert(effects, Effects.Count);
		}

		public bool Insert(AudioEffect effect, int index)
		{
			return Insert(new AudioEffect[] { effect }, index);
		}

		public bool Insert(IEnumerable<AudioEffect> effects, int index)
		{
			if (index >= 0 && index <= Effects.Count)
			{
				foreach (var effect in effects)
					_effects.Insert(index++, effect);
				ListUpdated(this, new ListUpdatedArgs(Effects));
				return true;
			}
			return false;
		}

		public bool Remove(AudioEffect effect)
		{
			return Remove(IndexOf(effect));
		}

		public bool Remove(int index)
		{
			if (index >= 0 && index < _effects.Count)
			{
				_effects.RemoveAt(index);
				ListUpdated(this, new ListUpdatedArgs(Effects));
				return true;
			}
			return false;
		}

		public virtual bool Move(AudioEffect effect, int targetIndex)
		{
			if (targetIndex <= Effects.Count && _effects.Remove(effect))
			{
				_effects.Insert(targetIndex, effect);
				ListUpdated(this, new ListUpdatedArgs(Effects));
				return true;
			}
			return false;
		}

		public virtual bool Move(AudioEffect source, AudioEffect target)
		{
			if (_effects.Contains(target) && _effects.Remove(source))
			{
				_effects.Insert(_effects.IndexOf(target), source);
				ListUpdated(this, new ListUpdatedArgs(Effects));
				return true;
			}
			return false;
		}

		public virtual bool Move(int sourceIndex, int targetIndex)
		{
			return Move(_effects[sourceIndex], targetIndex);
		}

		public int IndexOf(AudioEffect effect)
		{
			return _effects.IndexOf(effect);
		}

		public void Clear()
		{
			_effects.Clear();
			ListUpdated(this, new ListUpdatedArgs(Effects));
		}

		public override int AvailableSamples => _samples.Count;

		public override void Reset()
		{
			Clear();
		}

		public override void ClearState()
		{
			_samples.Clear();
		}

		public override void UpdateFormat(WaveFormat format)
		{
			foreach (var effect in Effects)
				effect.UpdateWith(format);
		}

		protected override void Put(Span<float> samples)
		{
			foreach (var effect in Effects)
				if (effect.Enabled)
					effect.Process(samples, CurrentFormat);
			for (int i = 0; i < samples.Length; i++)
				_samples.Enqueue(samples[i]);
		}

		protected override Span<float> Get(int count)
		{
			var array = new float[count];
			for (int i = 0; i < array.Length; i++)
				array[i] = _samples.Dequeue();
			return array;
		}

		public override void Deserialize(JContainer? obj)
		{
			_effects.Clear();
			if (obj != null)
			{
				var deserialized = obj.ToObject<EffectSerializeData[]?>();

				if (deserialized != null)
				{
					foreach (var deserializedEffect in deserialized)
					{
						var effectData = EffectDictionary.FindById(deserializedEffect.id);
						if (effectData != null)
						{
							var effect = EffectDictionary.CreateEffectInstance(effectData);
							if (effect != null)
							{
								effect.Enabled = deserializedEffect.enabled;
								effect.CustomPreset = deserializedEffect.customPreset;
								try
								{
									effectData.Presets.Select(effect, deserializedEffect.presetName, effectData.Attribute.DefaultPreset);
								}
								catch { }
								_effects.Add(effect);
							}
						}
					}

					ListUpdated(this, new ListUpdatedArgs(Effects));
				}
			}
		}

		public override JContainer Serialize()
		{
			var serializeList = new List<EffectSerializeData>();

			foreach (var effect in _effects)
			{
				var serializeData = new EffectSerializeData();
				var effectType = effect.GetType();

				serializeData.enabled = effect.Enabled;
				serializeData.presetName = effect.PresetName;
				serializeData.customPreset = effect.CustomPreset;
				serializeData.id = EffectDictionary.Dict.First(s => s.Type == effectType).Attribute.Id;

				serializeList.Add(serializeData);
			}
			return JArray.FromObject(serializeList.ToArray());
		}
	}
}