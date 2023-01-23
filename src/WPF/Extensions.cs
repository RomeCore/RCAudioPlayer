using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace RCAudioPlayer.WPF
{
	static public class Extensions
	{
		static public string Translate(this string key) => Translation.TranslationDictionary.Get(key);
		static public System.Windows.Data.BindingBase GetBinding(this string key) => Translation.TranslationListener.MakeBinding(key);

		public static List<Type> GetInheritanceHierarchy(this Type type)
		{
			List<Type> result = new List<Type>();
			for (var current = type; current != null; current = current.BaseType)
				result.Add(current);
			return result;
		}

		public static ContextMenu GetContextMenu(this Dictionary<string, Action> elements, ContextMenu? toAdd = null)
		{
			var menu = toAdd ?? new ContextMenu();

			foreach (var (name, click) in elements)
			{
				var menuItem = new MenuItem();
				menuItem.SetBinding(MenuItem.HeaderProperty, name.GetBinding());
				menuItem.Click += (s, e) => click();
				menu.Items.Add(menuItem);
			}

			return menu;
		}
	}
}