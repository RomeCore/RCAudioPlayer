using System;
using System.Windows.Input;
using MahApps.Metro.IconPacks;
using NHotkey.Wpf;
using RCAudioPlayer.Core.Playables;
using RCAudioPlayer.Core.Players;
using RCAudioPlayer.WPF.Playables;

namespace RCAudioPlayer.WPF.Players.Sub
{
	[PlayerSubControl(typeof(ElementPlayer))]
	public partial class ElementPlayerSubControl
	{
		private readonly bool hotkeysRegistered;

		private readonly Action playPause;
		private readonly Action prevTrack;
		private readonly Action nextTrack;

		public ElementPlayerSubControl(ElementPlayer elementPlayer) : base(elementPlayer)
		{
			InitializeComponent();
			subHolder.Content = new PlaybackPanelSubControl(elementPlayer.Master);

			playPause = () => elementPlayer.Master.Output.PlayPause();
			prevTrack = () => elementPlayer.Previous();
			nextTrack = () => elementPlayer.Next();

			playButton.Click += (s, e) => playPause();
			prevButton.Click += (s, e) => prevTrack();
			nextButton.Click += (s, e) => nextTrack();

			MainWindow.Current.TaskbarItemInfo.ThumbButtonInfos.Add(Utils.GetTaskbarButton(PackIconMaterialKind.Rewind, prevTrack));
			MainWindow.Current.TaskbarItemInfo.ThumbButtonInfos.Add(Utils.GetTaskbarButton(PackIconMaterialKind.PlayPause, playPause));
			MainWindow.Current.TaskbarItemInfo.ThumbButtonInfos.Add(Utils.GetTaskbarButton(PackIconMaterialKind.FastForward, nextTrack));

			elementPlayer.OnElementUpdate += ElementPlayer_OnElementUpdate;

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

		private void ElementPlayer_OnElementUpdate(object? sender, ElementPlayer.UpdateArgs<IPlayable> e)
		{
			playableHolder.Content = PlayableControlDictionary.GetSubFor(e.Playable, Player);
			if (App.Current.MainWindow != null)
				App.Current.MainWindow.Title = e.Playable.FullTitle;
		}
	}
}