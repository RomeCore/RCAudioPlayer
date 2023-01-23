using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using RCAudioPlayer.Core;
using RCAudioPlayer.Core.Effects;
using RCAudioPlayer.Core.Pipeline;
using RCAudioPlayer.WPF.Effects;

namespace RCAudioPlayer.WPF.Pipeline
{
	[PipelineElementControl(typeof(EffectChain))]
	public partial class EffectChainControl
	{
		public EffectChain EffectChain { get; }
		public int EffectsCount => Effects.Count;
		public IReadOnlyList<AudioEffect> Effects => EffectChain.Effects;

		private ListBoxItem MakeControl(AudioEffect effect)
		{
			var wrapper = new EffectWrapper(EffectControlDictionary.GetFor(effect));
			var effectListItem = new ListBoxItem { Content = wrapper };

			effectListItem.AllowDrop = true;
			wrapper.dragElement.Dragged += (s, e) => DragDrop.DoDragDrop(effectListItem, effectListItem, DragDropEffects.Move);
			effectListItem.DragOver += EffectListItem_DragOver;

			effectListItem.ContextMenu = new Dictionary<string, Action>
			{
				{ "delete", () => Remove(effect) }
			}.GetContextMenu();

			return effectListItem;
		}

		public EffectChainControl(EffectChain effectChain) : base(effectChain)
		{
			InitializeComponent();
			EffectChain = effectChain;

			controlsList.Items.Clear();
			foreach (var effect in effectChain.Effects)
				controlsList.Items.Add(MakeControl(effect));

			var addButtonMenu = new ContextMenu();
			addButton.ContextMenu = addButtonMenu;

			foreach (var data in EffectDictionary.Dict)
			{
				var menuItem = new MenuItem();
				menuItem.SetBinding(MenuItem.HeaderProperty, data.Attribute.Id.GetBinding());
				addButtonMenu.Items.Add(menuItem);
				menuItem.Click += (s, e) =>
				{
					var effect = EffectDictionary.CreateDefaultEffectInstance(data);
					if (effect != null)
						Add(effect);
				};
			}

			addButton.Click += (s, e) => addButtonMenu.IsOpen = true;
		}

		public bool Remove(int position)
		{
			if (EffectChain.Remove(position))
			{
				controlsList.Items.RemoveAt(position);
				return true;
			}
			return false;
		}
		
		public bool Remove(AudioEffect effect)
		{
			return Remove(EffectChain.IndexOf(effect));
		}
		
		public bool Insert(AudioEffect effect, int position)
		{
			if (position <= EffectsCount && position >= 0 && EffectChain.Insert(effect, position))
			{
				controlsList.Items.Insert(position, MakeControl(effect));
				return true;
			}
			return false;
		}

		public void Add(AudioEffect effect)
		{
			Insert(effect, EffectsCount);
		}
		
		public void AddFirst(AudioEffect effect)
		{
			Insert(effect, 0);
		}

		public bool Move(ListBoxItem source, int targetIndex)
		{
			if (controlsList.Items.Contains(source) && targetIndex >= 0 && targetIndex <= EffectsCount
				&& EffectChain.Move(((EffectWrapper)source.Content).EffectControl.AudioEffect, targetIndex))
			{
				controlsList.Items.Remove(source);
				controlsList.Items.Insert(targetIndex, source);
				return true;
			}
			return false;
		}

		public bool Move(int sourceIndex, int targetIndex)
		{
			if (sourceIndex >= 0 && sourceIndex < EffectsCount)
			{
				var source = (ListBoxItem)controlsList.Items[sourceIndex];
				return Move(source, targetIndex);
			}
			return false;
		}

		public bool Move(ListBoxItem source, ListBoxItem target)
		{
			return Move(controlsList.Items.IndexOf(source), controlsList.Items.IndexOf(target));
		}
		
		public bool Move(AudioEffect source, AudioEffect target)
		{
			return Move(EffectChain.IndexOf(source), EffectChain.IndexOf(target));
		}

		public void Clear()
		{
			EffectChain.Clear();
			controlsList.Items.Clear();

		}

		private void EffectListItem_DragOver(object sender, DragEventArgs e)
		{
			var sourceItem = (ListBoxItem)e.Data.GetData(typeof(ListBoxItem));
			var targetItem = (ListBoxItem)sender;

			if (sourceItem?.Content is EffectWrapper && sourceItem != targetItem)
				Move(sourceItem, targetItem);
			else
				throw new Exception();
		}
	}
}