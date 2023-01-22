using System;
using System.Media;
using System.Windows;

namespace RCAudioPlayer.WPF.Dialogs
{
	public partial class QuestionDialog
	{
		public QuestionDialog(string question, string title)
		{
			InitializeComponent();

			questionText.Text = question;
			Title = title;
		}

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			SystemSounds.Asterisk.Play();
		}

		private void YesButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}

		private void NoButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		public static bool? Show(string label, string title)
		{
			return new QuestionDialog(label, title).ShowDialog();
		}
	}
}