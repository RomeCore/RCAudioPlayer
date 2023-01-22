using System;
using System.IO;
using System.Collections.Generic;
using RCAudioPlayer.Core.Streams;
using NAudio.Wave;
using RCAudioPlayer.Core.Data;

namespace RCAudioPlayer.Core.Effects
{
	[UriSupports("mp3", "mp4", "wav", "ogg", "aif", "aiff")]
	public class TrackFile : IUriAudioData
	{
		private class StreamFileAbstraction : TagLib.File.IFileAbstraction
		{
			public string Name { get; }
			public Stream ReadStream { get; }
			public Stream WriteStream { get; }

			public StreamFileAbstraction(Stream target, string name)
			{
				Name = name;
				ReadStream = target;
				WriteStream = target;
			}

			public void CloseStream(Stream stream) => stream.Close();
		}

		public string Uri { get; }
		public Dictionary<string, object> Data { get; }
		public string Artist { get; }
		public string Title { get; }
		public float Length { get; }

		public TrackFile(string uri, Dictionary<string, object>? data = null)
		{
			Data = data ?? new Dictionary<string, object>();

			Uri = uri;
			var tagFile = TagLib.File.Create(new StreamFileAbstraction(NetClient.GetStream(Uri), uri));
			var uriAbstraction = new Uri(uri);

			if (uriAbstraction.IsFile)
			{
				var line = Path.GetFileNameWithoutExtension(uri);
				var parsed = Utils.ParseTitle(line);

				Artist = parsed[0];
				Title = parsed[1];
			}
			else
			{
				Artist = Data.Get("artist") as string ?? string.Empty;
				Title = Data.Get("title") as string ?? string.Empty;
				Length = Convert.ToSingle(Data.Get("length"));
			}

			var duration = tagFile.Properties.Duration;
			if (!string.IsNullOrWhiteSpace(tagFile.Tag.Title))
				Title = tagFile.Tag.Title;
			if (!string.IsNullOrWhiteSpace(tagFile.Tag.FirstPerformer))
				Artist = tagFile.Tag.FirstPerformer;
			if (duration != TimeSpan.Zero)
				Length = (float)duration.TotalSeconds;

			Data["uri"] = Uri;
			Data["artist"] = Artist;
			Data["title"] = Title;
			Data["length"] = Length;
		}

		public AudioInput GetInput() =>
			AudioInput.FromStream(new AudioFileReader(Uri), this);

		public Stream GetRawInput() => NetClient.GetStream(Uri);
	}
}