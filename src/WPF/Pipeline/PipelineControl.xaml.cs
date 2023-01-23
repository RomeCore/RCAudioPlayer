using System;
using System.Collections.Generic;
using System.Windows.Controls;
using RCAudioPlayer.Core.Pipeline;

namespace RCAudioPlayer.WPF.Pipeline
{
	public partial class PipelineControl : UserControl
	{
		public Core.Pipeline.Pipeline Pipeline { get; }
		public int ElementsCount => Pipeline.Elements.Count;

		private PipelineElementWrapper? MakeControl(PipelineElement element)
		{
			var control = PipelineElementControlDictionary.GetFor(element);
			if (control == null)
				return null;
			var wrapper = new PipelineElementWrapper(control);
			wrapper.ContextMenu = new Dictionary<string, Action>
			{
				{ "delete", () => Remove(element) }
			}.GetContextMenu();

			return wrapper;
		}

		public PipelineControl(Core.Pipeline.Pipeline pipeline)
		{
			InitializeComponent();
			Pipeline = pipeline;

			controlsList.Items.Clear();
			foreach (var element in pipeline.Elements)
				controlsList.Items.Add(MakeControl(element));

			var addButtonMenu = new ContextMenu();
			addButton.ContextMenu = addButtonMenu;

			foreach (var type in Core.Pipeline.Pipeline.Types)
			{
				var menuItem = new MenuItem();
				menuItem.SetBinding(MenuItem.HeaderProperty, type.Value.Attribute.Id.GetBinding());
				addButtonMenu.Items.Add(menuItem);
				menuItem.Click += (s, e) =>
				{
					var element = Core.Pipeline.Pipeline.Create(type.Key);
					if (element != null)
						Add(element);
				};
			}

			addButton.Click += (s, e) => addButtonMenu.IsOpen = true;
		}

		public bool Remove(int position)
		{
			if (Pipeline.Remove(position))
			{
				controlsList.Items.RemoveAt(position);
				return true;
			}
			return false;
		}

		public bool Remove(PipelineElement element)
		{
			return Remove(Pipeline.IndexOf(element));
		}

		public bool Insert(PipelineElement element, int position)
		{
			if (position <= ElementsCount && position >= 0 && Pipeline.Insert(element, position))
			{
				controlsList.Items.Insert(position, MakeControl(element));
				return true;
			}
			return false;
		}

		public void Add(PipelineElement element)
		{
			Insert(element, ElementsCount);
		}

		public void AddFirst(PipelineElement element)
		{
			Insert(element, 0);
		}

		public bool Move(PipelineElementWrapper source, int targetIndex)
		{
			if (controlsList.Items.Contains(source) && targetIndex >= 0 && targetIndex <= ElementsCount
				&& Pipeline.Move(source.PipelineElement, targetIndex))
			{
				controlsList.Items.Remove(source);
				controlsList.Items.Insert(targetIndex, source);
				return true;
			}
			return false;
		}

		public bool Move(int sourceIndex, int targetIndex)
		{
			if (sourceIndex >= 0 && sourceIndex < ElementsCount)
			{
				var source = (PipelineElementWrapper)controlsList.Items[sourceIndex];
				return Move(source, targetIndex);
			}
			return false;
		}

		public bool Move(PipelineElementWrapper source, PipelineElementWrapper target)
		{
			return Move(controlsList.Items.IndexOf(source), controlsList.Items.IndexOf(target));
		}

		public bool Move(PipelineElement source, PipelineElement target)
		{
			return Move(Pipeline.IndexOf(source), Pipeline.IndexOf(target));
		}

		public void Clear()
		{
			Pipeline.Clear();
			controlsList.Items.Clear();
		}
	}
}
