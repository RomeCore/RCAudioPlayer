using System;
using System.Media;
using System.Windows;
using MahApps.Metro.IconPacks;

namespace RCAudioPlayer.WPF.Dialogs
{
	public partial class MessageDialog
	{
		public MessageDialog(string message, PackIconMaterialKind iconKind = PackIconMaterialKind.Alert) : this(message, "message".Translate(), iconKind)
		{
		}
		
		public MessageDialog(string message, string title, PackIconMaterialKind iconKind = PackIconMaterialKind.Alert)
		{
			InitializeComponent();

			icon.Kind = iconKind;
			Title = title;
			messageText.Text = message;
		}

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
			SystemSounds.Asterisk.Play();
		}

        private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}

		public static bool? Show(string message, PackIconMaterialKind iconKind = PackIconMaterialKind.Alert)
        {
			return new MessageDialog(message, iconKind).ShowDialog();
        }

		public static bool? Show(string message, string title, PackIconMaterialKind iconKind = PackIconMaterialKind.Alert)
        {
			return new MessageDialog(message, title, iconKind).ShowDialog();
        }

		public static bool? Show(Exception exc)
        {
			return Show(exc.Message + '\n' + exc.StackTrace, "exception_msg".Translate());
        }
	}
}