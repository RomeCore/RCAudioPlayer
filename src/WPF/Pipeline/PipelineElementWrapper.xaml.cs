using System;
using System.Linq;
using System.Windows.Controls;
using RCAudioPlayer.Core.Pipeline;

namespace RCAudioPlayer.WPF.Pipeline
{
	public partial class PipelineElementWrapper : UserControl
	{
		public PipelineElementControl PipelineElementControl { get; }
		public PipelineElement PipelineElement { get; }

		public event EventHandler OnReset = (s, e) => { };

		public PipelineElementWrapper(PipelineElementControl elementControl)
		{
			InitializeComponent();
			PipelineElementControl = elementControl;
			PipelineElement = elementControl.PipelineElement;

			var elementType = PipelineElement.GetType();
			enabler.SetBinding(ContentControl.ContentProperty, 
				Core.Pipeline.Pipeline.Types.First(s => s.Value.Type == elementType).Key.GetBinding());
			enabler.IsChecked = PipelineElement.Enabled;
			enabler.Click += (s, e) => PipelineElement.Enabled = enabler.IsChecked ?? false;

			controlHolder.Content = elementControl;
		}

		private void ResetButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (Dialogs.QuestionDialog.Show("reset_sure".Translate(), "reset_sure_header".Translate()) ?? false)
			{
				OnReset(sender, e);
				PipelineElementControl.Reset();
			}
		}
	}
}