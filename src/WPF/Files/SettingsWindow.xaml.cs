using System.IO;
using System.Linq;
using System.Windows;
using RCAudioPlayer.WPF.Dialogs;
using RCAudioPlayer.WPF.Translation;

namespace RCAudioPlayer.WPF.Files
{
	public partial class SettingsWindow : Window
	{
		public SettingsWindow()
		{
			InitializeComponent();

			{ // no group

				// translation
				langSelector.ItemsSource = Directory.GetDirectories(TranslationDictionary.Folder).Select(s => System.IO.Path.GetFileName(s));
				langSelector.SelectedValue = Settings.Language;
				langSelector.SelectionChanged += (s, e) => Settings.Language = (string)langSelector.SelectedValue;

			} // no group

			{ // theme

				// configuration
				var dark_str = "theme_dark".Translate();
				var light_str = "theme_light".Translate();
				themeConfigSelector.Items.Add(dark_str);
				themeConfigSelector.Items.Add(light_str);
				themeConfigSelector.SelectedIndex = Settings.Theme == Theme.Dark ? 0 : 1;
				themeConfigSelector.SelectionChanged += (s, e) => 
					Settings.Theme = themeConfigSelector.SelectedIndex == 0 ? Theme.Dark : Theme.Light;

				// primary
				primaryColorButton.Click += (s, e) =>
				{
					var dialog = ColorDialog.Show("primary_color".Translate(), App.PrimaryColor);
					App.PrimaryColor = dialog.Color;
				};

				// secondary
				secondaryColorButton.Click += (s, e) =>
				{
					var dialog = ColorDialog.Show("secondary_color".Translate(), App.SecondaryThemeColor);
					App.SecondaryThemeColor = dialog.Color;
				};

			} // theme
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Settings.Save();
			Close();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Settings.Load();
			Close();
		}
	}
}