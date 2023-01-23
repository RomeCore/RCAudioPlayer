using RCAudioPlayer.Core.Playables;
using RCAudioPlayer.WPF.Dialogs;

namespace RCAudioPlayer.WPF.Playables.Main
{
	[PlayableControl(typeof(ErrorPlayable))]
	public partial class ErrorPlayableControl
	{
		public ErrorPlayableControl(ErrorPlayable playable) : base(playable)
		{
			InitializeComponent();

			stringText.Text = playable.String;
			if (playable.Exception != null)
				showErrorButton.Click += (s, e) => new ExceptionDialog(playable.Exception).ShowDialog();
			else
				showErrorButton.Visibility = System.Windows.Visibility.Collapsed;
		}
	}
}