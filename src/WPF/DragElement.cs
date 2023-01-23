using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RCAudioPlayer.WPF
{
	public class DragElement : Control
	{
		public static DependencyProperty BrushProperty = 
			DependencyProperty.Register("Brush", typeof(Brush), typeof(DragElement), new PropertyMetadata(Brushes.Gray));
		public static DependencyProperty LineThicknessProperty = 
			DependencyProperty.Register("LineThickness", typeof(double), typeof(DragElement), new PropertyMetadata(3d));
		public static DependencyProperty LineSpacingProperty = 
			DependencyProperty.Register("LineSpacing", typeof(double), typeof(DragElement), new PropertyMetadata(5d));

		public Brush Brush { get => (Brush)GetValue(BrushProperty); set => SetValue(BrushProperty, value); }
		public double LineThickness { get => (double)GetValue(LineThicknessProperty); set => SetValue(LineThicknessProperty, value); }
		public double LineSpacing { get => (double)GetValue(LineSpacingProperty); set => SetValue(LineSpacingProperty, value); }

		public event EventHandler Dragged = (s, e) => { };

		public DragElement()
		{

		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			var width = ActualWidth;
			var height = ActualHeight;
			var thickness = LineThickness;
			var spacing = LineSpacing + thickness;

			for (double offset = 0; offset < height; offset += spacing)
				drawingContext.DrawRectangle(Brush, null, new Rect(0, offset, width, thickness));

			base.OnRender(drawingContext);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
				Dragged(this, EventArgs.Empty);
			base.OnMouseMove(e);
		}
	}
}