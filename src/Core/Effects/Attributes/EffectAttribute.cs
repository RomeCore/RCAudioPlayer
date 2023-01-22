using System;

namespace RCAudioPlayer.Core.Effects.Attributes
{
    public class EffectAttribute : Attribute
    {
        public string Id { get; }
        public string DefaultPreset { get; }

        public EffectAttribute(string id, string defaultPreset)
        {
            Id = id;
            DefaultPreset = defaultPreset;
        }
    }
}