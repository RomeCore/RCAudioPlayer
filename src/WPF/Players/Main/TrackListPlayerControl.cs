using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MahApps.Metro.IconPacks;
using RCAudioPlayer.Core.Players;

namespace RCAudioPlayer.WPF.Players.Main
{
	[PlayerControl(typeof(TrackListPlayer))]
	public class TrackListPlayerControl : ListPlayerControl
	{
		protected override Dictionary<int, (PackIconMaterialKind, RoutedEventHandler, object?)>? Buttons =>
			new Dictionary<int, (PackIconMaterialKind, RoutedEventHandler, object?)>
			{
				{ 1, (PackIconMaterialKind.ClipboardOutline, AddClipbButton_Click, new Translation.TranslatedTooltip("add_clipboard")) }
			};

		public TrackListPlayerControl(ListPlayer listPlayer) : base(listPlayer)
		{
		}

		protected override void List_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
				var index = listBox.Items.IndexOf(sender);

				if (index != -1)
					Insert(files, index).ConfigureAwait(true);
				else
					Add(files).ConfigureAwait(true);
				e.Handled = true;
			}
		}

		protected override void AddButton_Click(object sender, RoutedEventArgs e)
		{
			var fileDialog = new Microsoft.Win32.OpenFileDialog
			{
				CheckFileExists = true,
				Multiselect = true
			};

			if (fileDialog.ShowDialog() ?? false)
				Add(fileDialog.FileNames).ConfigureAwait(true);
		}

		protected void AddClipbButton_Click(object sender, RoutedEventArgs e)
		{
			if (Clipboard.ContainsFileDropList())
				Add(Clipboard.GetFileDropList().Cast<string>()).ConfigureAwait(true);
		}
	}
}