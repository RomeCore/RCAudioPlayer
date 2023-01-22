using System;
using System.Windows.Controls;
using RCAudioPlayer.Core;
using RCAudioPlayer.Core.Effects;

namespace RCAudioPlayer.WPF.Effects
{
	public abstract class EffectControl : UserControl
	{
		public AudioEffect AudioEffect { get; }
		public EffectDictionary.Data AudioEffectData { get; }

		public EffectControl(AudioEffect effect) : this(effect, EffectDictionary.FindByType(effect.GetType())
				?? throw new NotSupportedException("This effect can't be control!"))
		{
		}
		
		public EffectControl(AudioEffect effect, EffectDictionary.Data effectData)
		{
			AudioEffect = effect;
			AudioEffectData = effectData;
		}

		static protected void ConfigureSlider(AudioEffect effect, Slider slider, Label? valueLabel, Action<double> setter, Func<double> getter)
		{
			var initValue = getter();
			slider.Value = initValue;
			if (valueLabel != null)
				valueLabel.Content = initValue.ToStr();

			bool updateEffect = true;
			slider.ValueChanged += (s, e) =>
			{
				if (valueLabel != null)
					valueLabel.Content = getter().ToStr();

				if (updateEffect)
				{
					effect.Update();
					setter(e.NewValue);
				}
				updateEffect = true;
			};
			effect.PresetChanged += (s, e) =>
			{
				updateEffect = false;
				slider.Value = getter();
			};
		}
	}
}