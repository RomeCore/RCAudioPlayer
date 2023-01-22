using System.ComponentModel;

namespace RCAudioPlayer.WPF
{
	public class Window : MahApps.Metro.Controls.MetroWindow
	{
		public bool IsButtonsEnabled { get 
			{
				return IsCloseButtonEnabled && IsMaxRestoreButtonEnabled && IsMinButtonEnabled;
			}
			set
			{
				IsCloseButtonEnabled = IsMaxRestoreButtonEnabled = IsMinButtonEnabled = value;
			}
		}
		
		public bool ShowButtons { get 
			{
				return ShowCloseButton || ShowMaxRestoreButton || ShowMinButton;
			}
			set
			{
				ShowCloseButton = ShowMaxRestoreButton = ShowMinButton = value;
			}
		}

		public Window()
		{
			BorderBrush = App.GlobalBorder;
			WindowTitleBrush = App.GlobalTitle;
			GlowBrush = App.GlobalGlow;

			App.BorderChanged += GlobalBorder_Changed;
			App.TitleChanged += GlobalTitle_Changed;
			App.GlowChanged += GlobalGlow_Changed;
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			App.BorderChanged -= GlobalBorder_Changed;
			App.TitleChanged -= GlobalTitle_Changed;
			App.GlowChanged -= GlobalGlow_Changed;
		}

		private void GlobalBorder_Changed(object? _, App.BrushChangedArgs args)
		{
			BorderBrush = args.Brush;
		}

		private void GlobalTitle_Changed(object? _, App.BrushChangedArgs args)
		{
			WindowTitleBrush = args.Brush;
		}

		private void GlobalGlow_Changed(object? _, App.BrushChangedArgs args)
		{
			GlowBrush = args.Brush;
		}
	}
}