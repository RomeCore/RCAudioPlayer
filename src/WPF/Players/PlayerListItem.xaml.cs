using System;
using System.Windows.Controls;
using RCAudioPlayer.Core;
using RCAudioPlayer.WPF.Management;
using MaterialDesignThemes.Wpf;
using MahApps.Metro.IconPacks;
using RCAudioPlayer.WPF.Players;
using RCAudioPlayer.WPF.Dialogs;

namespace RCAudioPlayer.WPF
{
	// That will be shown in players list
	public partial class PlayerListItem : UserControl
	{
		public PlayerManagerControl PlayerManagerControl { get; }
		public PlayerManager PlayerManager { get; }
		public PlayerManager.PlayerData PlayerData { get; }

		public PlayerListItem(PlayerManagerControl playerManagerControl, PlayerManager.PlayerData data)
		{
			InitializeComponent();
			PlayerManagerControl = playerManagerControl;
			PlayerManager = playerManagerControl.PlayerManager ?? throw new NullReferenceException("Player manager is null!");
			PlayerData = data;

			ButtonProgressAssist.SetIsIndeterminate(selectButton, true);
			nameText.Text = $"{PlayerData.Name}";

			editButton.Visibility = data.TypeData.Attribute.CanEdit
				? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
		}

		// This button loads player and opens player control (if it has it)
		private async void SelectButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var materialIcon = (PackIconMaterial)selectButton.Content;
			ButtonProgressAssist.SetIsIndicatorVisible(selectButton, true);
			try
			{
				await PlayerManagerControl.Load(PlayerData.Name);
				materialIcon.Kind = PackIconMaterialKind.Play;
			}
			catch (Exception exc)
			{
				Log.Exception(exc, "while loading player");
				materialIcon.Kind = PackIconMaterialKind.AlertCircleOutline;
				ExceptionDialog.Show(exc);
			}
			ButtonProgressAssist.SetIsIndicatorVisible(selectButton, false);
		}

		// This button shows rename dialog to rename player
		private void EditButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var content = PlayerManager.ExtractContent(PlayerData.Name);
			var dialog = PlayerControlDictionary.GetEditorFor(PlayerData.TypeData.Type, content);
			dialog.ShowDialog();

			if (dialog.Success)
				PlayerManager.ChangeContent(PlayerData.Name, dialog.Result);
		}
	}
}