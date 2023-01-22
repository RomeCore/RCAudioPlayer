using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using NAudio.Wave;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RCAudioPlayer.Core.Pipeline
{
	public class Pipeline
	{
		private struct ElementSerializeData
		{
			[JsonProperty("enabled")]
			public bool enabled;
			[JsonProperty("id")]
			public string id;
			[JsonProperty("data")]
			public JContainer? data;
		}

		public class ListUpdatedArgs
		{
			public IReadOnlyList<PipelineElement> Elements { get; }

			public ListUpdatedArgs(IReadOnlyList<PipelineElement> elements)
			{
				Elements = elements;
			}
		}

		public class Data
		{
			public Type Type { get; }
			public PipelineAttribute Attribute { get; }
			public ConstructorInfo Constructor { get; }

			public Data(Type type, PipelineAttribute attribute, ConstructorInfo constructor)
			{
				Type = type;
				Attribute = attribute;
				Constructor = constructor;
			}
		}

		public static IReadOnlyDictionary<string, Data> Types { get; }

		static public PipelineElement? Create(string id)
		{
			if (Types.TryGetValue(id, out var data))
            {
				var element = (PipelineElement)data.Constructor.Invoke(null);
				element.Enabled = true;
				return element;
			}
			return null;
		}

		static Pipeline()
		{
			var pipelineElementType = typeof(PipelineElement);
			Types = (from type in PluginManager.Types
					 where pipelineElementType.IsAssignableFrom(type)
					 let attribute = type.GetCustomAttribute<PipelineAttribute>()
					 where attribute != null
					 let constructor = type.GetConstructor(Type.EmptyTypes)
					 where constructor != null
					 select (type, attribute, constructor)).ToDictionary(k => k.attribute.Id, s => new Data(s.type, s.attribute, s.constructor));
		}

		private List<PipelineElement> _elements;
		private WaveFormat? _currentFormat;

		public IReadOnlyList<PipelineElement> Elements => _elements;
		public event EventHandler<ListUpdatedArgs> ListUpdated = (s, e) => { };

		public Pipeline()
		{
			_elements = new List<PipelineElement>();
		}

		public void Load(StreamReader reader)
		{
			var content = reader.ReadToEnd();
			var loaded = JsonConvert.DeserializeObject<ElementSerializeData[]?>(content);

			_elements.Clear();
			if (loaded != null)
				foreach (var loadedElement in loaded)
				{
					var element = Create(loadedElement.id);
					if (element != null)
					{
						element.Enabled = loadedElement.enabled;
						element.Deserialize(loadedElement.data);
						_elements.Add(element);
					}
				}
		}

		public void Save(StreamWriter writer)
		{
			var dataList = new List<ElementSerializeData>();

			foreach (var element in _elements)
			{
				try
				{
					var data = new ElementSerializeData();

					var elementType = element.GetType();
					data.enabled = element.Enabled;
					data.id = Types.First(s => s.Value.Type == elementType).Key;
					data.data = element.Serialize();

					dataList.Add(data);
				}
				catch
                {
                }
			}

			var content = JsonConvert.SerializeObject(dataList, Formatting.Indented);
			writer.Write(content);
		}

		public bool Insert(PipelineElement element, int index)
		{
			if (index >= 0 && index <= _elements.Count)
			{
				_elements.Insert(index, element);
				if (_currentFormat != null)
					element.UpdateFormat(_currentFormat);
				ListUpdated(this, new ListUpdatedArgs(Elements));
				return true;
			}
			return false;
		}

		public bool Add(PipelineElement element)
		{
			return Insert(element, _elements.Count);
		}

		public bool Remove(PipelineElement element)
		{
			return Remove(IndexOf(element));
		}

		public bool Remove(int index)
		{
			if (index >= 0 && index < _elements.Count)
			{
				_elements.RemoveAt(index);
				ListUpdated(this, new ListUpdatedArgs(Elements));
				return true;
			}
			return false;
		}

		public virtual bool Move(PipelineElement element, int targetIndex)
		{
			if (targetIndex <= _elements.Count && _elements.Remove(element))
			{
				_elements.Insert(targetIndex, element);
				ListUpdated(this, new ListUpdatedArgs(Elements));
				return true;
			}
			return false;
		}

		public virtual bool Move(PipelineElement source, PipelineElement target)
		{
			if (_elements.Contains(target) && _elements.Remove(source))
			{
				_elements.Insert(_elements.IndexOf(target), source);
				ListUpdated(this, new ListUpdatedArgs(Elements));
				return true;
			}
			return false;
		}

		public virtual bool Move(int sourceIndex, int targetIndex)
		{
			return Move(_elements[sourceIndex], targetIndex);
		}

		public int IndexOf(PipelineElement element)
		{
			return _elements.IndexOf(element);
		}

		public void Clear()
		{
			_elements.Clear();
			ListUpdated(this, new ListUpdatedArgs(Elements));
		}

		public void ClearState()
		{
			foreach (var element in _elements)
				element.ClearState();
		}

		public void UpdateFormat(WaveFormat format)
		{
			foreach (var element in _elements)
				element.UpdateFormat(format);
			_currentFormat = format;
		}

		public ReadOnlySpan<float> ReceiveSamples(int count, WaveStream inputStream)
		{
			var enabledElements = _elements.Where(s => s.Enabled).Prepend(new PipelineInputElement(inputStream));
			PipelineElement? inputElement = null;
			foreach (var element in enabledElements)
			{
				element.InputElement = inputElement;
				inputElement = element;
			}
			return enabledElements.Last().Receive(count);
		}

		public static ReadOnlySpan<float> ReceiveInputSamples(int count, WaveStream inputStream)
		{
			return PipelineInputElement.Receive(inputStream, count);
		}
	}
}