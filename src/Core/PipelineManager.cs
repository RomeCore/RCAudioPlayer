using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NAudio.Wave;

namespace RCAudioPlayer.Core
{
	public class PipelineManager
	{
		public class ChangedArgs
		{
			public string Name { get; }
			public string Filename { get; }
			public Pipeline.Pipeline? Pipeline { get; }

			public ChangedArgs(string name, Pipeline.Pipeline? pipeline)
			{
				Name = name;
				Filename = string.IsNullOrEmpty(name) ? string.Empty : GetFilename(name);
				Pipeline = pipeline;
			}
		}

		public class ListUpdatedArgs
		{
			public IEnumerable<string> List { get; }

			public ListUpdatedArgs(IEnumerable<string> list)
			{
				List = list;
			}
		}

		public static readonly string Folder = Files.UserFolder + "\\pipelines";
		public const string FileExtension = ".txt";
		public const string FilenamePattern = "*" + FileExtension;

		private static readonly Dictionary<string, string> pipelineFiles;

		public static IEnumerable<string> Pipelines => pipelineFiles.Keys;

		public static string GetFilename(string name)
		{
			return Path.Combine(Folder, name + FileExtension);
		}

		public static string GetName(string filename)
		{
			return Path.GetFileNameWithoutExtension(filename);
		}

		static PipelineManager()
		{
			Directory.CreateDirectory(Folder);
			pipelineFiles = Directory.EnumerateFiles(Folder, FilenamePattern)
				.ToDictionary(k => GetName(k), s => s);
		}

		private void Load(StreamReader reader)
		{
			_current = new Pipeline.Pipeline();
			_current.Load(reader);
			if (_prevFormat != null)
				_current.UpdateFormat(_prevFormat);
		}

		private void Save(StreamWriter writer)
		{
			if (_current == null)
				throw new InvalidOperationException("Can't save null pipeline!");

			_current.Save(writer);
		}

		private Pipeline.Pipeline? _previous;
		private Pipeline.Pipeline? _current;
		private WaveFormat? _prevFormat;

		private bool _enabled;

		public Pipeline.Pipeline? Current => _current;
		public string? CurrentName { get; private set; }
		public string? CurrentFilename { get; private set; }

		public bool Enabled { get => _enabled; set 
			{
				_enabled = value;
				if (!_enabled)
					Current?.ClearState();
			}
		}

		public event EventHandler<ChangedArgs> OnChanged = (s, e) => { };
		public event EventHandler<ListUpdatedArgs> OnListUpdated = (s, e) => { };

		public PipelineManager()
		{
			CurrentFilename = CurrentName = null;
			Enabled = true;
		}

		public bool Select(string name)
		{
			string filename = GetFilename(name);
			if (File.Exists(filename))
			{
				Save();

				CurrentFilename = filename;

				var reader = new StreamReader(CurrentFilename);
				Load(reader);
				reader.Close();

				CurrentName = name;
				OnChanged(this, new ChangedArgs(name, Current));

				return true;
			}
			return false;
		}

		private void _Deselect()
		{
			_current = null;
			CurrentName = string.Empty;
			OnChanged(this, new ChangedArgs(string.Empty, null));
		}

		public void Deselect()
		{
			Save();
			_Deselect();
		}

		public void Save()
		{
			if (Current != null && !string.IsNullOrEmpty(CurrentFilename))
			{
				var writer = new StreamWriter(CurrentFilename);
				Save(writer);
				writer.Close();
			}
		}

		public bool Add(string name, string basedOn = "")
		{
			if (name.Contains('\\') || name.Contains('/') || string.IsNullOrWhiteSpace(name))
				return false;

			var filename = GetFilename(name);
			if (pipelineFiles.TryAdd(name, filename))
			{
				if (!string.IsNullOrWhiteSpace(basedOn) && pipelineFiles.TryGetValue(basedOn, out var basedOnFilename))
				{
					if (CurrentFilename == basedOnFilename)
						Save();
					File.Copy(basedOnFilename, filename, true);
				}
				else
				{
					File.WriteAllText(filename, string.Empty);
				}

				OnListUpdated(this, new ListUpdatedArgs(Pipelines));
				return true;
			}
			return false;
		}

		public bool Remove(string name)
		{
			if (pipelineFiles.ContainsKey(name))
			{
				pipelineFiles.Remove(name);
				File.Delete(GetFilename(name));
				OnListUpdated(this, new ListUpdatedArgs(Pipelines));
				if (CurrentName == name)
					_Deselect();
				return true;
			}
			return false;
		}

		public ReadOnlySpan<float> ReceiveSamples(int count, WaveStream inputStream, WaveFormat outputFormat)
		{
			if (Enabled && _current != null)
			{
				if ((!_prevFormat?.Equals(outputFormat)) ?? true || _current != _previous)
					_current.UpdateFormat(outputFormat);
				_previous = _current;
				_prevFormat = outputFormat;
				return _current.ReceiveSamples(count, inputStream);
			}
			return Pipeline.Pipeline.ReceiveInputSamples(count, inputStream);
		}
	}
}