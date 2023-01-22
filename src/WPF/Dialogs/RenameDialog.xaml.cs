using System.Windows;

namespace RCAudioPlayer.WPF.Dialogs
{
	public partial class RenameDialog
	{
		public string StartText { get; }
		public string Text { get => textEditor.Text; }

		public RenameDialog(string startInput = "")
		{
			InitializeComponent();

			textEditor.Text = StartText = startInput;
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = StartText != Text && !string.IsNullOrWhiteSpace(Text);
			Close();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		public static RenameDialog Show(string startInput = "")
		{
			var dialog = new RenameDialog(startInput);
			dialog.ShowDialog();
			return dialog;
		}
	}
}