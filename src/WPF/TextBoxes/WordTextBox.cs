using System.Text.RegularExpressions;

namespace RCAudioPlayer.WPF.TextBoxes
{
	public class WordTextBox : FilterTextBox
	{
		public WordTextBox()
		{
			Filter = new Regex(@"\w+");
		}
	}
}