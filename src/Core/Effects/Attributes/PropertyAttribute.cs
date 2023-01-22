using System;

namespace RCAudioPlayer.Core.Effects.Attributes
{
	public abstract class PropertyAttribute : Attribute
	{
        public string Name { get; }
        public int Order { get; }

		protected PropertyAttribute(string name, int order)
        {
            Name = name;
            Order = order;
        }
    }
}