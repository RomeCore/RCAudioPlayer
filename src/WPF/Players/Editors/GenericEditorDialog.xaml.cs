using System.Windows;

namespace RCAudioPlayer.WPF.Players.Editors
{
	public partial class GenericEditorDialog
	{
		public override string Result => textEditor.Text;
		public override bool Success => !string.IsNullOrWhiteSpace(Result);

		public GenericEditorDialog(string startingResult) : base(startingResult)
		{
			InitializeComponent();

			textEditor.Text = StartingResult;
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}