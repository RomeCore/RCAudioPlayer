using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using RCAudioPlayer.Core.Players;
using RCAudioPlayer.WPF.Playables;
using MahApps.Metro.IconPacks;
using RCAudioPlayer.WPF.Translation;

namespace RCAudioPlayer.WPF.Players.Main
{
	public abstract class ListPlayerControl : PlayerControl
	{
		public ListBox listBox;
		public Button backButton;
		public TextBlock playerNameText;

		public ListPlayer ListPlayer { get; }
		public int Count => Playlist.Count;
		public IReadOnlyList<IPlayable> Playlist => ListPlayer.Playlist;

		protected virtual Dictionary<int, (PackIconMaterialKind, RoutedEventHandler, object?)>? Buttons { get; }

		protected virtual ListBoxItem MakeControl(IPlayable playable)
		{
			var wrapper = new PlayableWrapper(PlayableControlDictionary.GetFor(playable, ListPlayer), ListPlayer);
			var playableItem = new ListBoxItem { Content = wrapper };

			playableItem.AllowDrop = true;
			wrapper.dragElement.Dragged += (s, e) => DragDrop.DoDragDrop(playableItem, playableItem, DragDropEffects.Move);
			playableItem.DragOver += PlayableItem_DragOver;
			playableItem.Drop += List_Drop;

			playableItem.ContextMenu = new Dictionary<string, Action>
			{
				{ "delete", () => Remove(playable) }
			}.GetContextMenu();

			return playableItem;
		}

		private void PlayableItem_DragOver(object sender, DragEventArgs e)
		{
			var sourceItem = (ListBoxItem)e.Data.GetData(typeof(ListBoxItem));
			var targetItem = (ListBoxItem)sender;

			if (sourceItem?.Content is PlayableWrapper && sourceItem != targetItem)
				Move(sourceItem, targetItem);
		}

		public ListPlayerControl(ListPlayer listPlayer) : base(listPlayer)
		{
			#region init
			var dockPanel = new DockPanel { HorizontalAlignment = HorizontalAlignment.Stretch };

			var listBoxItem = new ListBoxItem { HorizontalContentAlignment = HorizontalAlignment.Stretch };
            {
                var buttonsList = Buttons ?? new Dictionary<int, (PackIconMaterialKind, RoutedEventHandler, object?)>();
                buttonsList.TryAdd(0, (PackIconMaterialKind.Plus, AddButton_Click, new TranslatedTooltip("add.")));
                buttonsList.TryAdd(10, (PackIconMaterialKind.DeleteSweepOutline, DeleteSelectionButton_Click, new TranslatedTooltip("remove_selected")));
				int buttonsCount = buttonsList.Count;

                var grid = new Grid();
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
				for (int i = 0; i < buttonsCount; i++)
					grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

				backButton = new Button
				{
					Content = new PackIconMaterial { Kind = PackIconMaterialKind.ArrowLeft },
					Style = (Style)App.Current.Resources["MaterialDesignFloatingActionMiniButton"]
				};
				grid.Children.Add(backButton);
				Grid.SetColumn(backButton, 0);

				playerNameText = new TextBlock();
				var label = new Label { Content = playerNameText, VerticalAlignment = VerticalAlignment.Center };
				grid.Children.Add(label);
				Grid.SetColumn(label, 1);

                int offset = 0;
				foreach (var buttonInfo in buttonsList.OrderBy(s => s.Key))
				{
					var button = new Button
					{
						Margin = new Thickness(3, 0, buttonsList.Count == offset - 1 ? 0 : 3, 0),
						Content = new PackIconMaterial { Kind = buttonInfo.Value.Item1 },
						Style = (Style)App.Current.Resources["MaterialDesignFloatingActionMiniButton"]
					};
					if (buttonInfo.Value.Item3 != null)
						button.ToolTip = buttonInfo.Value.Item3;
					button.Click += buttonInfo.Value.Item2;
					grid.Children.Add(button);
					Grid.SetColumn(button, 2 + offset);
					offset++;
				}

				listBoxItem.Content = grid;
			}
			dockPanel.Children.Add(listBoxItem);
			DockPanel.SetDock(listBoxItem, Dock.Top);

			var splitter = new GridSplitter
			{
				Height = 3,
				IsEnabled = false,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Stretch
			};
			dockPanel.Children.Add(splitter);
			DockPanel.SetDock(splitter, Dock.Top);

			listBox = new ListBox();
			dockPanel.Children.Add(listBox);
			DockPanel.SetDock(listBox, Dock.Bottom);

			Content = dockPanel;
			#endregion

			AllowDrop = true;
			ListPlayer = listPlayer;

			listBox.SelectionMode = SelectionMode.Extended;
			listBox.Items.Clear();
			foreach (var playable in Playlist)
				listBox.Items.Add(MakeControl(playable));

			backButton.Click += (s, e) => BackAction();
			playerNameText.Text = listPlayer.Name;
		}

		public bool Insert(IPlayable playable, int position)
		{
			if (listBox == null)
				throw new NullReferenceException("ListBox is null!");
			if (position <= Count && position >= 0 && ListPlayer.Insert(playable, position))
			{
				listBox.Items.Insert(position, MakeControl(playable));
				return true;
			}
			return false;
		}

		public void Insert(IEnumerable<IPlayable> playables, int position)
		{
			foreach (var playable in playables)
				Insert(playable, position++);
		}

		public async Task Insert(IEnumerable<string> lines, int position)
		{
			var added = ListPlayer.Insert(lines, position);
			await foreach (var playable in added)
				listBox.Items.Insert(position++, MakeControl(playable));
		}

		public async Task Add(IEnumerable<string> lines)
		{
			await Insert(lines, listBox.Items.Count);
		}

		public void Add(IPlayable playable)
		{
			Insert(playable, Count);
		}

		public void Add(IEnumerable<IPlayable> playables)
		{
			Insert(playables, Count);
		}

		public void AddFirst(IPlayable playable)
		{
			Insert(playable, 0);
		}

		public void Remove(IPlayable playable)
		{
			var index = ListPlayer.IndexOf(playable);
			if (ListPlayer.Remove(index))
				listBox.Items.RemoveAt(index);
		}

		public void Remove(ListBoxItem playableItem)
		{
			var playableControl = (PlayableWrapper)playableItem.Content;
			Remove(playableControl.PlayableControl.Playable);
		}

		public bool Move(ListBoxItem source, int targetIndex)
		{
			if (listBox.Items.Contains(source) && targetIndex >= 0 && targetIndex <= Count
				&& ListPlayer.Move(((PlayableWrapper)source.Content).PlayableControl.Playable, targetIndex))
			{
				listBox.Items.Remove(source);
				listBox.Items.Insert(targetIndex, source);
				return true;
			}
			return false;
		}

		public bool Move(int sourceIndex, int targetIndex)
		{
			if (sourceIndex >= 0 && sourceIndex < Count)
			{
				var source = (ListBoxItem)listBox.Items[sourceIndex];
				return Move(source, targetIndex);
			}
			return false;
		}

		public bool Move(ListBoxItem source, ListBoxItem target)
		{
			return Move(listBox.Items.IndexOf(source), listBox.Items.IndexOf(target));
		}

		public bool Move(IPlayable source, int targetIndex)
		{
			return Move(ListPlayer.IndexOf(source), targetIndex);
		}

		public bool Move(IPlayable source, IPlayable target)
		{
			return Move(ListPlayer.IndexOf(source), ListPlayer.IndexOf(target));
		}

		protected abstract void List_Drop(object sender, DragEventArgs e);
		protected abstract void AddButton_Click(object sender, RoutedEventArgs e);

		private void DeleteSelectionButton_Click(object sender, RoutedEventArgs e)
		{
			var selectedItems = listBox.SelectedItems.Cast<ListBoxItem>().ToList();
			foreach (var selectedItem in selectedItems)
				Remove(selectedItem);
		}
	}
}