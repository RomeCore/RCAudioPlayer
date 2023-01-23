using System;
using System.Reflection;
using System.Windows.Controls;
using RCAudioPlayer.Core;
using RCAudioPlayer.Core.Effects;
using RCAudioPlayer.Core.Effects.Attributes;

namespace RCAudioPlayer.WPF.Effects.Properties
{
	[EffectPropertyControl(typeof(RangeAttribute))]
	public class RangePropertyControl : EffectPropertyControl
	{
		public RangePropertyControl(AudioEffect effect, PropertyInfo info, RangeAttribute attribute) : base(effect, info, attribute)
		{
			var container = new StackPanel { Orientation = Orientation.Horizontal };
			var label = new Label();
			var slider = new Slider();
			container.Children.Add(label);
			container.Children.Add(slider);

			Content = container;

			label.VerticalAlignment = System.Windows.VerticalAlignment.Center;
			label.Content = info.GetValue(effect).ToStr();
			slider.VerticalAlignment = System.Windows.VerticalAlignment.Center;
			slider.Width = 120;

			slider.Minimum = attribute.Min;
			slider.Maximum = attribute.Max;
			slider.Value = (double)(Convert.ChangeType(info.GetValue(effect), typeof(double)) ?? throw new Exception());

			if (attribute.TickFrequency != 0)
			{
				slider.IsSnapToTickEnabled = true;
				slider.TickFrequency = attribute.TickFrequency;
			}

			bool update = true;
			slider.ValueChanged += (s, e) =>
			{
				if (update)
				{
					info.SetValue(effect, Convert.ChangeType(slider.Value, info.PropertyType));
					effect.Update();
				}
				label.Content = info.GetValue(effect).ToStr(3);
				update = true;
			};
			effect.PresetChanged += (s, e) =>
			{
				update = false;
				slider.Value = (double)(Convert.ChangeType(info.GetValue(effect), typeof(double)) ?? throw new Exception());
			};
		}
	}
}