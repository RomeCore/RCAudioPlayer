using System;
using System.Windows;
using System.Windows.Controls;

namespace RCAudioPlayer.WPF.TextBoxes
{
	public class TextBoxButton : Grid
	{
		public class EnterArgs
		{
			public string Text { get; }

			public EnterArgs(string text)
			{
				Text = text;
			}
		}

		public static readonly DependencyProperty ButtonContentProperty = 
			DependencyProperty.Register("ButtonContent", typeof(object), typeof(TextBoxButton), new PropertyMetadata(OnButtonContentChangedCallBack));

		private static void OnButtonContentChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			TextBoxButton? tb = sender as TextBoxButton;
			if (tb != null)
				tb.ButtonContent = e.NewValue;
		}

		public Button Button { get; }
		public TextBox TextBox { get; }

		public object ButtonContent { get => Button.Content; set => Button.Content = value; }
		public string Text { get => TextBox.Text; set => TextBox.Text = value; }

		public event EventHandler<EnterArgs> Entered = (s, e) => { };

		public TextBoxButton()
		{
			Button = new Button();
			Children.Add(Button);
			
			TextBox = new WordTextBox { Visibility = Visibility.Collapsed };
			Children.Add(TextBox);

			Button.Click += (s, e) =>
			{
				Button.Visibility = Visibility.Collapsed;
				TextBox.Visibility = Visibility.Visible;
				TextBox.Focus();
			};

			void EnableButton()
			{
				Button.Visibility = Visibility.Visible;
				TextBox.Visibility = Visibility.Collapsed;
				TextBox.Text = string.Empty;
			}

			TextBox.KeyDown += (s, e) =>
			{
				if (e.Key == System.Windows.Input.Key.Enter)
				{
					if (TextBox.Text != string.Empty)
						Entered(this, new EnterArgs(TextBox.Text));
					EnableButton();
				}
			};

			TextBox.LostFocus += (s, e) => EnableButton();
		}
	}
}