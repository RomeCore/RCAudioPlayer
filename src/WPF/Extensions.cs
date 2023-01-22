using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using MahApps.Metro.IconPacks;

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

		public static ContextMenu GetContextMenu(this Dictionary<string, Action> elements)
        {
			var menu = new ContextMenu();

			foreach (var (name, click) in elements)
			{
				var menuItem = new MenuItem();
				menuItem.SetBinding(MenuItem.HeaderProperty, name.GetBinding());
				menuItem.Click += (s, e) => click();
				menu.Items.Add(menuItem);
			}

			return menu;
		}

		private static Dictionary<PackIconMaterialKind, ImageSource> IconDictionary 
			= new Dictionary<PackIconMaterialKind, ImageSource>();

		public static ImageSource GetImageSource(this PackIconMaterialKind iconKind)
		{
			if (IconDictionary.TryGetValue(iconKind, out var imageSource))
				return imageSource;

			var icon = new PackIconMaterial { Kind = iconKind };
			var geo = Geometry.Parse(icon.Data);
			var gd = new GeometryDrawing();

			gd.Geometry = geo;
			gd.Brush = Brushes.White;

			var geoImage = new DrawingImage(gd);
			geoImage.Freeze();
			IconDictionary.Add(iconKind, geoImage);
			return geoImage;
		}

		public static Image GetImage(this PackIconMaterialKind iconKind)
		{
			return new Image { Source = iconKind.GetImageSource() };
		}

		public static BitmapImage GetUrlBitmap(this string url)
		{
			BitmapImage bitmap = new BitmapImage();
			bitmap.BeginInit();
			bitmap.UriSource = new Uri(url, UriKind.Absolute);
			bitmap.EndInit();
			return bitmap;
		}

		public static ThumbButtonInfo GetTaskbarButton(this PackIconMaterialKind iconKind, Action press)
        {
			var button = new ThumbButtonInfo();
			button.ImageSource = iconKind.GetImageSource();
			button.Click += (s, e) => press();
			return button;
		}
	}
}