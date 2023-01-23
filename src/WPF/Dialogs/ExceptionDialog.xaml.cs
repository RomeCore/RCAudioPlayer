using System;

namespace RCAudioPlayer.WPF.Dialogs
{
	public partial class ExceptionDialog
	{
		public ExceptionDialog(Exception exc)
		{
			InitializeComponent();

			messageText.Text = exc.Message;
			stackTraceText.Text = exc.StackTrace;
		}

		public static void Show(Exception exc)
		{
			new ExceptionDialog(exc).ShowDialog();
		}
	}
}