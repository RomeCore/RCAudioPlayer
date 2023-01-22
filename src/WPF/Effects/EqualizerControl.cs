using System;
using System.Windows;
using System.Windows.Controls;
using RCAudioPlayer.Core;
using RCAudioPlayer.Core.Effects;

namespace RCAudioPlayer.WPF.Effects
{
	[EffectControl(typeof(EqualizerEffect))]
	public class EqualizerControl : EffectControl
	{
		public EqualizerControl(EqualizerEffect effect) : base(effect)
		{
			Load(effect, AudioEffectData);
		}

		public EqualizerControl(EqualizerEffect effect, EffectDictionary.Data effectData) : base(effect, effectData)
		{
			Load(effect, AudioEffectData);
		}

		private void Load(EqualizerEffect equalizer, EffectDictionary.Data effectData)
		{
			var grid = new Grid();

			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
			grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(70) });
			grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(70) });
			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

			int maxGain = 25;
			int minGain = -25;

			var gainTop = new Label { Content = maxGain, VerticalAlignment = VerticalAlignment.Top };
			grid.Children.Add(gainTop);
			Grid.SetRow(gainTop, 0);

			var gainBottom = new Label { Content = minGain, VerticalAlignment = VerticalAlignment.Bottom };
			grid.Children.Add(gainBottom);
			Grid.SetRow(gainBottom, 1);

			var gainTextLabel = new Label
			{
				Content = "Gain",
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center
			};
			grid.Children.Add(gainTextLabel);
			Grid.SetRow(gainTextLabel, 2);

			var fqTextLabel = new Label
			{
				Content = "FQ",
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center
			};
			grid.Children.Add(fqTextLabel);
			Grid.SetRow(fqTextLabel, 3);

			int column = 1;
			foreach (var band in equalizer.Bands)
			{
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

				var gainLabel = new Label
				{
					VerticalAlignment = VerticalAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center
				};
				var slider = new Slider
				{
					Orientation = Orientation.Vertical,
					HorizontalAlignment = HorizontalAlignment.Center,
					Minimum = minGain,
					Maximum = maxGain
				};
				ConfigureSlider(equalizer, slider, gainLabel, s => band.Gain = (float)s, () => band.Gain);

				grid.Children.Add(slider);
				Grid.SetColumn(slider, column);
				Grid.SetRow(slider, 0);
				Grid.SetRowSpan(slider, 2);

				grid.Children.Add(gainLabel);
				Grid.SetColumn(gainLabel, column);
				Grid.SetRow(gainLabel, 2);

				var fqLabel = new Label
				{
					Content = band.Frequency,
					VerticalAlignment = VerticalAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center
				};
				grid.Children.Add(fqLabel);
				Grid.SetColumn(fqLabel, column);
				Grid.SetRow(fqLabel, 3);

				column++;
			}

			Content = grid;
		}
	}
}