using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace RCAudioPlayer.WPF.TextBoxes
{
    public class FilterTextBox : TextBox
    {
        public Regex? Filter { get; set; }

        public FilterTextBox()
        {
            Style = (Style)App.Current.Resources[typeof(TextBox)];
        }

        protected virtual bool MatchingFilter(string text)
        {
            return Filter != null && Filter.IsMatch(text);
        }

        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            if (MatchingFilter(e.Text))
                base.OnTextInput(e);
        }
    }
}