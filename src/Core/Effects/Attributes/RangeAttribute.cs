namespace RCAudioPlayer.Core.Effects.Attributes
{
	public class RangeAttribute : PropertyAttribute
	{
		public double Min { get; }
		public double Max { get; }
		public double TickFrequency { get; }

		public RangeAttribute(string name, double min, double max, double tickFq = 0, int order = 0) : base(name, order)
		{
			Min = min;
			Max = max;
			TickFrequency = tickFq;
		}
	}
}