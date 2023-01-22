using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace RCAudioPlayer.WPF.Translation
{
	public class TranslateExtension : MarkupExtension
	{
		[ConstructorArgument("key")]
		public string Key { get; set; }

		public TranslateExtension(string key)
		{
			Key = key;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return MakeBinding().ProvideValue(serviceProvider);
		}

		public BindingBase MakeBinding()
		{
			return TranslationListener.MakeBinding(Key);
		}
	}
}