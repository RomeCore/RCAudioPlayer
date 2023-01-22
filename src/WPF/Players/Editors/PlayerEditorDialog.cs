namespace RCAudioPlayer.WPF.Players.Editors
{
	public abstract class PlayerEditorDialog : Window
	{
		public string StartingResult { get; }

		public abstract string Result { get; }
		public abstract bool Success { get; }

		public PlayerEditorDialog(string startingResult = "")
		{
			StartingResult = startingResult;
		}
	}
}