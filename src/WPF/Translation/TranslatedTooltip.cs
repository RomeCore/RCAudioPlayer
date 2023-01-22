using System.Windows;
using System.Windows.Controls;

namespace RCAudioPlayer.WPF.Translation
{
	public class TranslatedTooltip : ToolTip
	{
		public TranslatedTooltip(string key)
		{
			Style = (Style)App.Current.Resources[typeof(ToolTip)];
			var binding = key.GetBinding();
			SetBinding(ContentProperty, binding);
		}
	}
}