using System;
using System.Windows.Data;
using System.Windows.Controls;
using RCAudioPlayer.Core;
using RCAudioPlayer.Core.Effects;

namespace RCAudioPlayer.WPF.Effects
{
	public partial class EffectWrapper : UserControl
	{
		private static readonly string DisplayCustom;

		static EffectWrapper()
		{
			DisplayCustom = "Custom";
		}

		public EffectControl EffectControl { get; }
		public AudioEffect AudioEffect { get; }
		public EffectDictionary.Data AudioEffectData { get; }
		public EffectDictionary.PresetCollection PresetCollection { get; }

		public EffectWrapper(EffectControl control)
		{
			InitializeComponent();
			EffectControl = control;
			AudioEffect = EffectControl.AudioEffect;
			AudioEffectData = EffectControl.AudioEffectData;

			controlHolder.Content = control;

			effectEnabler.SetBinding(ContentProperty, AudioEffectData.Attribute.Id.GetBinding());
			effectEnabler.IsChecked = AudioEffect.Enabled;
			effectEnabler.Checked += (s, e) => AudioEffect.Enabled = true;
			effectEnabler.Unchecked += (s, e) => AudioEffect.Enabled = false;

			PresetCollection = EffectDictionary.GetPresetsFor(AudioEffect.GetType()) ??
				throw new NullReferenceException("Can't find presets for this effect type");

			presetSelector.Items.Add(DisplayCustom);
			foreach (var preset in PresetCollection.Presets)
				presetSelector.Items.Add(preset.Key);

			SwitchToPreset(AudioEffect.PresetName);
			presetSelector.SelectionChanged += (s, e) =>
			{
				SwitchToPreset((string)presetSelector.SelectedValue);
			};
			AudioEffect.Updated += (s, e) =>
			{
				AudioEffect.CustomPreset = AudioEffect.SerializePreset();
				SwitchToCustom();
			};

			createButton.Entered += (s, e) =>
			{
				var name = e.Text;
				if (!AudioEffectData.Presets.Presets.ContainsKey(name))
					presetSelector.Items.Add(name);
				AudioEffectData.Presets.Set(name, AudioEffect.SerializePreset());
				AudioEffect.PresetName = name;
				presetSelector.SelectedValue = name;
			};
		}

		public void SwitchToPreset(string presetName)
		{
			if (string.IsNullOrEmpty(presetName))
				SwitchToDefault();
			else if (presetName == EffectDictionary.PresetCollection.CustomPresetName || presetName == DisplayCustom)
				SwitchToCustom();
			else
			{
				PresetCollection.Select(AudioEffect, presetName, AudioEffectData.Attribute.DefaultPreset);
				if (presetSelector.Items.IndexOf(presetName) != -1)
					presetSelector.SelectedValue = presetName;
				else
					presetSelector.SelectedIndex = 0;
			}
		}

		public void SwitchToCustom()
		{
			if (AudioEffect.PresetName != EffectDictionary.PresetCollection.CustomPresetName)
				PresetCollection.Select(AudioEffect, EffectDictionary.PresetCollection.CustomPresetName, AudioEffectData.Attribute.DefaultPreset);
			presetSelector.SelectedIndex = 0;
		}

		public void SwitchToDefault()
		{
			if (AudioEffectData.Attribute.DefaultPreset != AudioEffect.SerializePreset())
				PresetCollection.Select(AudioEffect, string.Empty, AudioEffectData.Attribute.DefaultPreset);
			presetSelector.SelectedIndex = 0;
		}
	}
}