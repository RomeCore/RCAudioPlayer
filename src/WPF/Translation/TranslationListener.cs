using System;
using System.Windows.Data;
using System.ComponentModel;

namespace RCAudioPlayer.WPF.Translation
{
	public class TranslationListener : INotifyPropertyChanged
	{
		public string Key { get; }
		public string StringValue => TranslationDictionary.Get(Key);
		public object Value => StringValue;

		public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };

		public TranslationListener(string key)
		{
			Key = key;
			TranslationDictionary.LanguageChanged += OnLanguageChanged;
		}

		~TranslationListener()
		{
			TranslationDictionary.LanguageChanged -= OnLanguageChanged;
		}

		private void OnLanguageChanged(object? sender, EventArgs e)
		{
			PropertyChanged(this, new PropertyChangedEventArgs("Value"));
		}

		public BindingBase MakeBinding()
		{
			return new Binding("Value")
			{
				Source = new TranslationListener(Key)
			};
		}

		public static BindingBase MakeBinding(string key)
		{
			return new TranslationListener(key).MakeBinding();
		}
	}
}