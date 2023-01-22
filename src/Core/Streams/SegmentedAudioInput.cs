using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using NAudio.Wave;
using RCAudioPlayer.Core.Data;

namespace RCAudioPlayer.Core.Streams
{
	public class SegmentedAudioInput : AudioInput
	{
		public List<IAudioData> Segments { get; }

		public override WaveFormat WaveFormat { get; }
		public override long Length => _combinedStream.Length;
		public override long Position { get => _combinedStream.Position; set => _combinedStream.Position = value; }

		private CombinedStream _combinedStream;

		public SegmentedAudioInput(List<IAudioData> segments, IAudioData source) : base(source)
		{
			if (segments.Count < 1)
				throw new ArgumentException("Segments list must contain at least one element!", nameof(segments));
			Segments = segments;

			var tasks = segments.ConvertAll(s => Task.Run(() => s.GetInput()));
			var list = tasks.ConvertAll(s => s.Result);
			_combinedStream = new CombinedStream(list);
			WaveFormat = list[0].WaveFormat;
		}

		public override int Read(byte[] buffer, int offset, int count) => _combinedStream.Read(buffer, offset, count);
	}
}