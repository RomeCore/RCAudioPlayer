using System.Windows;
using System.Windows.Controls;
using RCAudioPlayer.Core;
using RCAudioPlayer.Core.Effects;

namespace RCAudioPlayer.WPF.Effects
{
    public class GenericEffectControl : EffectControl
	{
		public GenericEffectControl(AudioEffect effect) : base(effect)
		{
			Load(effect, AudioEffectData);
		}

		public GenericEffectControl(AudioEffect effect, EffectDictionary.Data effectData) : base(effect, effectData)
		{
			Load(effect, AudioEffectData);
		}

		private void Load(AudioEffect effect, EffectDictionary.Data effectData)
		{
			var properties = effectData.Properties;
			var grid = new Grid();

			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

			int row = 0;
			foreach (var property in properties)
			{
				var propertyControl = EffectControlDictionary.GetPropertyFor(effect, property.Key, property.Value);
				if (propertyControl != null)
				{
					grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

					var nameLabel = new Label();
					nameLabel.SetBinding(ContentProperty, property.Value.Name.GetBinding());
					nameLabel.VerticalAlignment = VerticalAlignment.Center;
					grid.Children.Add(nameLabel);
					Grid.SetRow(nameLabel, row);
					Grid.SetColumn(nameLabel, 0);

					propertyControl.VerticalAlignment = VerticalAlignment.Center;
					propertyControl.HorizontalAlignment = HorizontalAlignment.Right;
					grid.Children.Add(propertyControl);
					Grid.SetColumn(propertyControl, 1);
					Grid.SetRow(propertyControl, row);

					row++;
				}
			}

			Content = grid;
		}
	}
}