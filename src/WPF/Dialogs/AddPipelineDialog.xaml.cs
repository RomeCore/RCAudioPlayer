using System.Windows;
using RCAudioPlayer.Core;

namespace RCAudioPlayer.WPF.Dialogs
{
	public partial class AddPipelineDialog
	{
		public string EffectChainName => nameEditor.Text;
		public string BasedOn => basedOnSelector.Text;

		public AddPipelineDialog()
		{
			InitializeComponent();
			basedOnSelector.ItemsSource = PipelineManager.Pipelines;
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = !string.IsNullOrWhiteSpace(EffectChainName);
			Close();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		public static new AddPipelineDialog Show()
		{
			var dialog = new AddPipelineDialog();
			dialog.ShowDialog();
			return dialog;
		}
	}
}
