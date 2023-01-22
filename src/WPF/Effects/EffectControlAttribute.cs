using System;

namespace RCAudioPlayer.WPF.Effects
{
	public class EffectControlAttribute : Attribute
	{
		public Type EffectType { get; }

		public EffectControlAttribute(Type effectType)
        {
            if (!typeof(Core.Effects.AudioEffect).IsAssignableFrom(effectType))
                throw new Exception("This type can't be assigned to AudioEffect type");
            EffectType = effectType;
        }
    }
}