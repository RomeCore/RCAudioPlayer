using System.Windows;
using System.Windows.Media;

namespace RCAudioPlayer.WPF.Dialogs
{
	public partial class ColorDialog : Window
	{
		public Color StartColor { get; set; }
		public Color Color => picker.Color;

		public ColorDialog(string title, Color color)
		{
			InitializeComponent();

			Title = title;
			StartColor = picker.Color = color;
		}
		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = StartColor != Color;
			Close();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		public static ColorDialog Show(string title, Color color)
		{
			var dialog = new ColorDialog(title, color);
			dialog.ShowDialog();
			return dialog;
		}
	}
}