using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace RCAudioPlayer.WPF.TextBoxes
{
	public class NumericTextBox : FilterTextBox
	{
		public double Value { get => double.Parse(Text); set => Text = value.ToString().Replace('.', ','); }

		public NumericTextBox()
		{
			Filter = new Regex("[0-9,]+");
		}

		protected override bool MatchingFilter(string text)
		{
			if (base.MatchingFilter(text))
				return !Text.Contains(',') || !text.Contains(',');
			return false;
		}

		protected override void OnTextChanged(TextChangedEventArgs e)
		{
			if (string.IsNullOrEmpty(Text))
				Text = "0";
			base.OnTextChanged(e);
		}
	}
}