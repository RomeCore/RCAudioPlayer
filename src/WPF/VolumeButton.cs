using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace RCAudioPlayer.WPF
{
	public class VolumeButton : Button
	{
		private Slider _volumeSlider;
		private Popup _sliderPopup;

		public double SliderHeight { get => _volumeSlider.Height; set => _volumeSlider.Height = value; }
		public double Volume { get => _volumeSlider.Value; set => _volumeSlider.Value = value; }
		public event RoutedPropertyChangedEventHandler<double> VolumeChanged = (s, e) => { };

		public VolumeButton()
		{
			Style = (Style)App.Current.Resources[typeof(Button)];

			var grid = new Grid();
			SizeChanged += (s, e) => grid.Width = ActualWidth;

			_volumeSlider = new Slider
			{
				Orientation = Orientation.Vertical,
				Minimum = 0,
				Maximum = 100,
				Margin = new Thickness(10),
				AutoToolTipPlacement = AutoToolTipPlacement.TopLeft,
				AutoToolTipPrecision = 0,
				HorizontalAlignment = HorizontalAlignment.Center
			};
			_volumeSlider.ValueChanged += (s, e) => VolumeChanged(s, e);
			grid.Children.Add(_volumeSlider);

			_sliderPopup = new Popup
			{
				PlacementTarget = this,
				PopupAnimation = PopupAnimation.Fade,
				Placement = PlacementMode.Top,
				Child = grid,
				StaysOpen = false,
				AllowsTransparency = true
			};

			SliderHeight = 150;

			Click += (s, e) => _sliderPopup.IsOpen = !_sliderPopup.IsOpen;
			MouseWheel += (s, e) => Volume += e.Delta / 100.0;
		}
	}
}