using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using RCAudioPlayer.Core;
using RCAudioPlayer.Core.Players;
using RCAudioPlayer.WPF.Players;
using System.Threading.Tasks;

namespace RCAudioPlayer.WPF.Management
{
	public partial class PlayerManagerControl
	{
		public PlayerManager? PlayerManager { get; private set; }
		public string? PlayerName { get; private set; }
		public Player? CurrentPlayer { get; private set; }
		public PlayerControl? CurrentControl { get; private set; }

		private ListBoxItem GetPlayerItem(PlayerManager.PlayerData data)
		{
			var item = new PlayerListItem(this, data);
			var listItem = new ListBoxItem { Content = item, HorizontalContentAlignment = HorizontalAlignment.Stretch };

			item.renameButton.Click += (s, e) =>
			{
				var dialog = new Dialogs.RenameDialog(data.Name);
				if (dialog.ShowDialog() ?? false)
					PlayerManager?.Move(data.Name, dialog.Text);
			};
			item.deleteButton.Click += (s, e) =>
			{
				if (Dialogs.QuestionDialog.Show($"{"remove_sure_frag".Translate()} {data.Name}?", "remove_sure_header".Translate()) ?? false)
					PlayerManager?.Remove(data.Name);
			};

			return listItem;
		}

		public PlayerManagerControl()
		{
			InitializeComponent();
			SwitchToList();

			addButton.Click += AddButton_Click;
			showPlayerButton.Click += (s, e) => SwitchToPlayer();
			showPlayerButton.Visibility = Visibility.Collapsed;
		}

		public void Load(PlayerManager playerManager)
		{
			Load();

			PlayerManager = playerManager;
			void UpdateList()
			{
				playerList.Items.Clear();
				foreach (var player in PlayerManager.Players)
					playerList.Items.Add(GetPlayerItem(player.Value));
			}
			UpdateList();
			PlayerManager.OnListUpdated += (s, e) => UpdateList();

			var startPlayer = Files.State.Get("player", string.Empty);
			if (!string.IsNullOrWhiteSpace(startPlayer) && PlayerManager.PlayersList.Contains(startPlayer))
				try
				{
					Load(startPlayer).ConfigureAwait(true);
				}
				catch
				{

				}
		}

		private void SwitchToList()
		{
			listPanel.Visibility = Visibility.Visible;
			playerPanel.Visibility = Visibility.Collapsed;
		}

		private void SwitchToPlayer()
		{
			playerPanel.Visibility = Visibility.Visible;
			listPanel.Visibility = Visibility.Collapsed;
		}

		public async Task Load(string playerName)
		{
			if (PlayerManager == null)
				throw new InvalidOperationException("Player manager is not loaded!");

			MainWindow.Current.TaskbarItemInfo.ThumbButtonInfos.Clear();
			if (await PlayerManager.Load(playerName))
			{
				showPlayerButton.Visibility = Visibility.Visible;

				Files.State.Set("player", playerName);
				PlayerName = playerName;
				CurrentPlayer = PlayerManager.Current ?? throw new Exception();
				CurrentControl = PlayerControlDictionary.GetFor(CurrentPlayer);
				playerPanel.Content = CurrentControl;

				if (CurrentControl != null)
				{
					CurrentControl.BackAction += SwitchToList;
					SwitchToPlayer();
				}
				else
				{
					CurrentPlayer.Play();
				}
			}
			else
			{
				SwitchToList();
				throw new Exception("Can't load player!");
			}
		}

		private void AddButton_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new Dialogs.AddPlayerDialog();
			if (dialog.ShowDialog() ?? false)
				PlayerManager?.Add(dialog.PlayerName, dialog.PlayerType, dialog.EditResult);
		}
	}
}