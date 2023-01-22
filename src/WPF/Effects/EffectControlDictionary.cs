using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using RCAudioPlayer.Core;
using RCAudioPlayer.Core.Effects;
using RCAudioPlayer.Core.Effects.Attributes;
using RCAudioPlayer.WPF.Effects.Properties;

namespace RCAudioPlayer.WPF.Effects
{
    public static class EffectControlDictionary
    {
        static private Dictionary<Type, ConstructorInfo> Specials { get; }
        static private Dictionary<Type, ConstructorInfo> PropertySpecials { get; }

        static private Dictionary<AudioEffect, EffectControl> Dictionary { get; }

        static EffectControlDictionary()
        {
            Dictionary = new Dictionary<AudioEffect, EffectControl>();

            var effectControlType = typeof(EffectControl);
            var effectDataType = typeof(EffectDictionary.Data);
            Specials = (from type in PluginManager.Types
                        where effectControlType.IsAssignableFrom(type)
                        let attribute = type.GetCustomAttribute<EffectControlAttribute>()
                        where attribute != null
                        let constructor = type.GetConstructor(new Type[] { attribute.EffectType, effectDataType })
                             ?? type.GetConstructor(new Type[] { attribute.EffectType })
                        where constructor != null
                        select (attribute.EffectType, constructor))
                        .ToDictionary(k => k.Item1, s => s.constructor);

            var audioEffectType = typeof(AudioEffect);
            var propertyInfoType = typeof(PropertyInfo);
            var effectPropertyControlType = typeof(EffectPropertyControl);
            PropertySpecials = (from type in PluginManager.Types
                                where effectPropertyControlType.IsAssignableFrom(type)
                                let attribute = type.GetCustomAttribute<EffectPropertyControlAttribute>()
                                where attribute != null
                                let constructor = type.GetConstructor(new Type[] { audioEffectType, propertyInfoType, attribute.PropertyAttributeType })
                                where constructor != null
                                select (attribute.PropertyAttributeType, constructor))
                               .ToDictionary(k => k.Item1, s => s.constructor);
        }

        static private void RegisterInstance(AudioEffect effect, EffectControl control)
        {
            Dictionary.Add(effect, control);
            effect.OnDelete += (s, e) => Dictionary.Remove(effect);
        }

        static public EffectControl GetFor(AudioEffect effect)
        {
            EffectControl? control;
            if (Dictionary.TryGetValue(effect, out control))
                return control;

            var effectType = effect.GetType();
            var effectData = EffectDictionary.FindByType(effectType) ?? throw new NotSupportedException("This effect can't be control!");

            if (Specials.TryGetValue(effect.GetType(), out var constructor))
            {
                if (constructor.GetParameters().Length == 2)
                    control = (EffectControl)constructor.Invoke(new object[] { effect, effectData });
                else
                    control = (EffectControl)constructor.Invoke(new object[] { effect });
                RegisterInstance(effect, control);
                return control;
            }

            control = new GenericEffectControl(effect, effectData);
            RegisterInstance(effect, control);
            return control;
        }

        static public EffectPropertyControl? GetPropertyFor(AudioEffect effect, PropertyInfo info, PropertyAttribute attribute)
        {
            if (PropertySpecials.TryGetValue(attribute.GetType(), out var constructor))
                return (EffectPropertyControl)constructor.Invoke(new object[] { effect, info, attribute });
            return null;
        }
    }
}