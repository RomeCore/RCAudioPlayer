using System;
using System.Windows;
using System.Runtime.InteropServices;
using System.ComponentModel;
using RCAudioPlayer.Core;
using RCAudioPlayer.WPF.Files;
using System.Windows.Input;

namespace RCAudioPlayer.WPF
{
	public partial class MainWindow
	{
		[DllImport("kernel32")]
		private static extern void AllocConsole();
		[DllImport("kernel32")]
		private static extern void FreeConsole();

		public static bool Active { get; private set; }
#pragma warning disable CS8618
		public static MainWindow Current { get; private set; }
		public static PlayerManager PlayerManager { get; }

		static MainWindow()
		{
			PlayerManager = new PlayerManager();
		}
#pragma warning restore CS8618

		public MainWindow()
		{
			InitializeComponent();

			Current = this;
			TaskbarItemInfo = new System.Windows.Shell.TaskbarItemInfo();

			Activated += (s, e) => Active = true;
			Deactivated += (s, e) => Active = false;

			var windowLeft = State.Get("window_left", -1.0);
			if (windowLeft != -1.0)
				Left = windowLeft;
			var windowTop = State.Get("window_top", -1.0);
			if (windowTop != -1.0)
				Top = windowTop;
			var windowWidth = State.Get("window_width", -1.0);
			if (windowWidth != -1.0)
				Width = windowWidth;
			var windowHeight = State.Get("window_height", -1.0);
			if (windowHeight != -1.0)
				Height = windowHeight;
			var windowState = (WindowState)State.Get("window_state", (int)WindowState.Normal);
			if (windowState != WindowState.Normal)
				WindowState = windowState;

			var playerManagerSize = State.Get("playerm_size", -1.0);
			if (playerManagerSize != -1.0)
				leftColumn.Width = new GridLength(playerManagerSize);
			var pipelineManagerSize = State.Get("pipelinem_size", -1.0);
			if (pipelineManagerSize != -1.0)
				rightColumn.Width = new GridLength(pipelineManagerSize);

			playbackPanel.Load(PlayerManager);
			playerManagerControl.Load(PlayerManager);
			pipelineManagerControl.Load(PlayerManager.Master.PipelineManager);
			visualizerManagerControl.Load(PlayerManager.Master);

			//

			PlayerManager.Master.Volume = State.Get("volume", 0.07f);
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Space && PlayerManager.Current != null)
			{
				PlayerManager.Current.PlayPause();
				e.Handled = true;
			}
			base.OnPreviewKeyDown(e);
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			State.Set("window_left", Left);
			State.Set("window_top", Top);
			State.Set("window_width", Width);
			State.Set("window_height", Height);
			State.Set("window_state", WindowState);

			State.Set("playerm_size", leftColumn.Width.Value);
			State.Set("pipelinem_size", rightColumn.Width.Value);

			PlayerManager.Dispose();
            State.Save();
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			Environment.Exit(0);
		}

		private void SettingsButton_Click(object sender, RoutedEventArgs e)
		{
			new SettingsWindow().ShowDialog();
		}
	}
}