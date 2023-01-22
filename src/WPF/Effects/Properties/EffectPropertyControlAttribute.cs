using System;

namespace RCAudioPlayer.WPF.Effects.Properties
{
	public class EffectPropertyControlAttribute : Attribute
	{
		public Type PropertyAttributeType { get; }

		public EffectPropertyControlAttribute(Type propertyAttributeType)
		{
			PropertyAttributeType = propertyAttributeType;
		}
	}
}