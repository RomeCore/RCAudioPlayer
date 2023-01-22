using System.Linq;
using System.Windows;
using RCAudioPlayer.Core;
using RCAudioPlayer.WPF.Players;

namespace RCAudioPlayer.WPF.Dialogs
{
    public partial class AddPlayerDialog
	{
		private string _selectedType;

		public bool CanEdit { get; private set; }
		public string PlayerName => nameEditor.Text;
		public string PlayerType => _selectedType;
		public string EditResult { get; private set; }
		public bool EditSuccess { get; private set; }

		public AddPlayerDialog()
		{
			InitializeComponent();

			CanEdit = false;
			EditResult = string.Empty;
			editButton.Visibility = Visibility.Collapsed;

			var types = PlayerManager.Types;
			_selectedType = string.Empty;

            foreach (var type in types.Keys)
				typeSelector.Items.Add(type.Translate());

			typeSelector.SelectionChanged += (s, e) =>
            {
				int index = typeSelector.SelectedIndex;
                var type = types.ElementAt(index);
                _selectedType = type.Key;
				
				if (CanEdit = type.Value.Attribute.CanEdit)
					editButton.Visibility = Visibility.Visible;
				else
					editButton.Visibility = Visibility.Collapsed;
            };

		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = !string.IsNullOrWhiteSpace(PlayerName) && !string.IsNullOrWhiteSpace(PlayerType)
				&& (!CanEdit || EditSuccess);
			Close();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		public static new AddPlayerDialog Show()
		{
			var dialog = new AddPlayerDialog();
			dialog.ShowDialog();
			return dialog;
		}

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
			if (PlayerManager.Types.TryGetValue(PlayerType, out var playerTypeData))
            {
				var dialog = PlayerControlDictionary.GetEditorFor(playerTypeData.Type, EditResult);
				if (dialog.ShowDialog() ?? false)
                {
					EditResult = dialog.Result;
					EditSuccess = dialog.Success;
				}
			}
        }
    }
}