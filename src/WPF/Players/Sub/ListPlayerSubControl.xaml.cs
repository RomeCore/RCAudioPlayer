using System;
using System.Windows.Input;
using MahApps.Metro.IconPacks;
using NHotkey.Wpf;
using RCAudioPlayer.Core.Players;
using RCAudioPlayer.WPF.Playables;

namespace RCAudioPlayer.WPF.Players.Sub
{
    [PlayerSubControl(typeof(ListPlayer))]
	public partial class ListPlayerSubControl
	{
		private static bool shuffle;
		private static bool repeat;

		private readonly bool hotkeysRegistered;

		private readonly Action playPause;
		private readonly Action prevTrack;
		private readonly Action nextTrack;
		private readonly Action<bool> shuffleList;
		private readonly Action<bool> repeatTrack;

		static ListPlayerSubControl()
		{
			shuffle = Files.State.Get("shuffle", false);
            repeat = Files.State.Get("repeat", false);
        }

        public ListPlayerSubControl(ListPlayer listPlayer) : base(listPlayer)
		{
			InitializeComponent();
			subHolder.Content = new PlaybackPanelSubControl(listPlayer.Master);

			playPause = () => listPlayer.PlayPause();
			prevTrack = () => listPlayer.Previous();
			nextTrack = () => listPlayer.Next();

			shuffleList = (s) => Files.State.Set("shuffle", shuffle = listPlayer.Shuffle = s);
			repeatTrack = (l) => Files.State.Set("repeat", repeat = listPlayer.Loop = l);

			playButton.Click += (s, e) => playPause();
			prevButton.Click += (s, e) => prevTrack();
			nextButton.Click += (s, e) => nextTrack();

            MainWindow.Current.TaskbarItemInfo.ThumbButtonInfos.Add(PackIconMaterialKind.Rewind.GetTaskbarButton(prevTrack));
            MainWindow.Current.TaskbarItemInfo.ThumbButtonInfos.Add(PackIconMaterialKind.PlayPause.GetTaskbarButton(playPause));
            MainWindow.Current.TaskbarItemInfo.ThumbButtonInfos.Add(PackIconMaterialKind.FastForward.GetTaskbarButton(nextTrack));

            shuffleButton.IsChecked = listPlayer.Shuffle = shuffle;
			shuffleButton.Click += (s, e) => shuffleList(shuffleButton.IsChecked ?? false);
			repeatButton.IsChecked = listPlayer.Loop = repeat;
			repeatButton.Click += (s, e) => repeatTrack(repeatButton.IsChecked ?? false);

			listPlayer.OnElementUpdate += ListPlayer_OnElementUpdate;

			try
			{
				hotkeysRegistered = true;

				HotkeyManager.Current.AddOrReplace("PlayPause", Key.MediaPlayPause,
					ModifierKeys.None, (s, e) => playPause());

				HotkeyManager.Current.AddOrReplace("Prev_Track", Key.MediaPreviousTrack,
					ModifierKeys.None, (s, e) => prevTrack());

				HotkeyManager.Current.AddOrReplace("Next_Track", Key.MediaNextTrack,
					ModifierKeys.None, (s, e) => nextTrack());
			}
			catch (Exception)
			{
				hotkeysRegistered = false;
				Dialogs.MessageDialog.Show("hotkey_alert".Translate(), "hotkey_alert_header".Translate());
			}

		}

		public override void OnClose()
		{
			if (hotkeysRegistered)
			{
				HotkeyManager.Current.Remove("PlayPause");
				HotkeyManager.Current.Remove("Prev_Track");
				HotkeyManager.Current.Remove("Next_Track");
			}
			((PlaybackPanelSubControl)subHolder.Content).OnClose();
		}

		private void ListPlayer_OnElementUpdate(object? sender, ElementPlayer.UpdateArgs<IPlayable> e)
		{
			playableHolder.Content = PlayableControlDictionary.GetSubFor(e.Playable, Player);
			if (App.Current.MainWindow != null)
				App.Current.MainWindow.Title = e.Playable.FullTitle;
		}
	}
}