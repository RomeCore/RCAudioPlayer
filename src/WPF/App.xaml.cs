using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ControlzEx.Theming;
using MaterialDesignThemes.Wpf;
using RCAudioPlayer.WPF.Files;

namespace RCAudioPlayer.WPF
{
	public enum Theme
	{
		Light,
		Dark
	}

	public partial class App : Application
	{
		public class BrushChangedArgs
		{
			public Brush Brush { get; }

			public BrushChangedArgs(Brush brush)
			{
				Brush = brush;
			}
		}
		
		public class ColorChangedArgs
		{
			public Color Color { get; }

			public ColorChangedArgs(Color color)
			{
				Color = color;
			}
		}

		private static PaletteHelper paletteHelper;

		private static Theme theme;
		private static Brush borderBrush;
		private static Brush titleBrush;
		private static Brush glowBrush;
		private static Color primaryThemeColor;
		private static Color primaryColor;
		private static Color secondaryThemeColor;

		public static event EventHandler<BrushChangedArgs> BorderChanged = (s, e) => { };
		public static event EventHandler<BrushChangedArgs> TitleChanged = (s, e) => { };
		public static event EventHandler<BrushChangedArgs> GlowChanged = (s, e) => { };
		public static event EventHandler<ColorChangedArgs> PrimaryChanged = (s, e) => { };
		public static event EventHandler<ColorChangedArgs> SecondaryChanged = (s, e) => { };

		public static void UpdateThemeConfig()
		{
			var theme = paletteHelper.GetTheme();
			switch (App.theme)
			{
				case Theme.Light:
					theme.SetBaseTheme(new MaterialDesignLightTheme());
					ThemeManager.Current.ChangeTheme(Current, "Light.Steel");
					break;
				case Theme.Dark:
					theme.SetBaseTheme(new MaterialDesignDarkTheme());
					ThemeManager.Current.ChangeTheme(Current, "Dark.Steel");
					break;
			}
			paletteHelper.SetTheme(theme);
		}

		public static Theme Theme { get => theme; set
			{
				theme = value;
				UpdateThemeConfig();
			}
		}

		public static Brush GlobalBorder { get => borderBrush; set 
			{
				borderBrush = value;
				BorderChanged(null, new BrushChangedArgs(value));
			}
		}
		
		public static Brush GlobalTitle { get => titleBrush; set 
			{
				titleBrush = value;
				TitleChanged(null, new BrushChangedArgs(value));
			}
		}
		
		public static Brush GlobalGlow { get => glowBrush; set 
			{
				glowBrush = value;
				GlowChanged(null, new BrushChangedArgs(value));
			}
		}

		public static Color PrimaryThemeColor { get => primaryThemeColor; set => SetPrimaryThemeColor(value); }
		public static Color PrimaryColor { get => primaryColor; set => SetPrimaryColor(value); }

		public static Color SecondaryThemeColor { get => secondaryThemeColor; set => SetSecondaryThemeColor(value); }

		static App()
		{
			paletteHelper = new PaletteHelper();

			theme = Theme.Dark;
			borderBrush = Brushes.Black;
			titleBrush = borderBrush;
			glowBrush = borderBrush;
		}

		public App()
		{
			DispatcherUnhandledException += App_DispatcherUnhandledException;
		}

		private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			Core.Log.Exception(e.Exception);
			var exc = e.Exception;
			Dialogs.MessageDialog.Show($"Exception occured! Message: {exc.Message}\nStack trace: {exc.StackTrace}");
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			Translation.TranslationDictionary.LoadLanguage("ru");
			Settings.Load();
		}

		static private void _SetPrimaryThemeColor(Color color)
		{
			var theme = paletteHelper.GetTheme();
			theme.SetPrimaryColor(color);
			paletteHelper.SetTheme(theme);
			primaryThemeColor = color;
			PrimaryChanged(null, new ColorChangedArgs(color));
		}
		
		static public void SetPrimaryThemeColor(Color color)
		{
			_SetPrimaryThemeColor(color);
			UpdateThemeConfig();
		}

		static private void _SetSecondaryThemeColor(Color color)
		{
			var theme = paletteHelper.GetTheme();
			theme.SetSecondaryColor(color);
			paletteHelper.SetTheme(theme);
			secondaryThemeColor = color;
			SecondaryChanged(null, new ColorChangedArgs(color));
		}
		
		static public void SetSecondaryThemeColor(Color color)
		{
			_SetSecondaryThemeColor(color);
			UpdateThemeConfig();
		}

		static private void _SetPrimaryColor(Color color)
		{
			GlobalBorder = new SolidColorBrush(color);
			GlobalTitle = new SolidColorBrush(color);
			GlobalGlow = new SolidColorBrush(color);
			primaryColor = color;
			SetPrimaryThemeColor(color);
		}

		static public void SetPrimaryColor(Color color)
		{
			_SetPrimaryColor(color);
			UpdateThemeConfig();
		}
		
		static public void Configure(Color primary, Color secondary, Theme theme)
		{
			_SetPrimaryColor(primary);
			_SetSecondaryThemeColor(secondary);
			Theme = theme;
		}
	}
}