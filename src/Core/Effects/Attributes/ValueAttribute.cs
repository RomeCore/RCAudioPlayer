namespace RCAudioPlayer.Core.Effects.Attributes
{
	public class ValueAttribute : PropertyAttribute
	{
		public ValueAttribute(string name, int order = 0) : base(name, order)
		{
		}
	}
}