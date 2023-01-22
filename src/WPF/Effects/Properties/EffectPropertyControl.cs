using System.Reflection;
using System.Windows.Controls;
using RCAudioPlayer.Core.Effects;
using RCAudioPlayer.Core.Effects.Attributes;

namespace RCAudioPlayer.WPF.Effects.Properties
{
	public class EffectPropertyControl : UserControl
	{
		public AudioEffect AudioEffect { get; }
		public PropertyInfo PropertyInfo { get; }
		public PropertyAttribute PropertyAttribute { get; }

		public EffectPropertyControl(AudioEffect audioEffect, PropertyInfo propertyInfo, PropertyAttribute propertyAttribute)
		{
			AudioEffect = audioEffect;
			PropertyInfo = propertyInfo;
			PropertyAttribute = propertyAttribute;
		}
	}
}