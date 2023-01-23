using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using MahApps.Metro.IconPacks;

namespace RCAudioPlayer.WPF
{
	public static class Utils
	{
		#region icon to image

		private static Dictionary<PackIconMaterialKind, ImageSource> IconDictionary
			= new Dictionary<PackIconMaterialKind, ImageSource>();

		public static ImageSource GetImageSource(PackIconMaterialKind iconKind)
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

		public static Image GetImage(PackIconMaterialKind iconKind)
		{
			return new Image { Source = GetImageSource(iconKind) };
		}

		public static ThumbButtonInfo GetTaskbarButton(PackIconMaterialKind iconKind, Action press)
		{
			var button = new ThumbButtonInfo();
			button.ImageSource = GetImageSource(iconKind);
			button.Click += (s, e) => press();
			return button;
		}

		#endregion

		#region data to image 

		public static BitmapSource GetBitmapFromUri(string uri)
		{
			BitmapImage bitmap = new BitmapImage();
			bitmap.BeginInit();
			bitmap.UriSource = new Uri(uri, UriKind.Absolute);
			bitmap.EndInit();
			return bitmap;
		}
		
		public static BitmapSource GetBitmapFromBytes(byte[] data)
		{
			BitmapImage bitmap = new BitmapImage();
			bitmap.BeginInit();
			bitmap.StreamSource = new MemoryStream(data);
			bitmap.EndInit();
			return bitmap;
		}

		public static BitmapSource GetBitmapFromRawBytes(byte[] bytes, int sizeX, int sizeY)
		{
			return BitmapSource.Create(sizeX, sizeY, 300, 300, PixelFormats.Pbgra32, BitmapPalettes.Gray256, bytes, 2);
		}

		// Use if image is square form
		public static BitmapSource GetBitmapFromRawBytes(byte[] bytes)
		{
			int pixels = bytes.Length / 4;
			int size = (int)Math.Sqrt(pixels);
			return BitmapSource.Create(size, size, 300, 300, PixelFormats.Pbgra32, BitmapPalettes.Gray256, bytes, 2);
		}

		#endregion
	}
}