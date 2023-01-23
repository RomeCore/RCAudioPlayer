using System;
using System.Reflection;
using RCAudioPlayer.Core.Effects;
using RCAudioPlayer.Core.Effects.Attributes;
using RCAudioPlayer.WPF.TextBoxes;

namespace RCAudioPlayer.WPF.Effects.Properties
{
	[EffectPropertyControl(typeof(ValueAttribute))]
	public class ValuePropertyControl : EffectPropertyControl
	{
		public ValuePropertyControl(AudioEffect effect, PropertyInfo info, ValueAttribute attribute) : base(effect, info, attribute)
		{
			var textBox = new NumericTextBox();
			Content = textBox;

			bool update = true;
			textBox.Value = (double)(Convert.ChangeType(info.GetValue(effect), typeof(double)) ?? throw new Exception());
			textBox.TextChanged += (s, e) =>
			{
				info.SetValue(effect, Convert.ChangeType(textBox.Value, info.PropertyType));
				if (update)
					effect.Update();
				update = true;
			};
			effect.PresetChanged += (s, e) =>
			{
				update = false;
				textBox.Value = (double)(Convert.ChangeType(info.GetValue(effect), typeof(double)) ?? throw new Exception());
			};
		}
	}
}