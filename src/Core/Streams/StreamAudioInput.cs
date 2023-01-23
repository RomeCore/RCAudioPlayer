using System;
using System.IO;
using NAudio.Wave;
using RCAudioPlayer.Core.Data;

namespace RCAudioPlayer.Core.Streams
{
	// AudioInput class that works with raw readable streams
	public class StreamAudioInput : AudioInput
	{
		public Stream SourceStream { get; }

		public override WaveFormat WaveFormat { get; }
		public override long Length => SourceStream.Length;
		public override long Position { get => SourceStream.Position; set => SourceStream.Position = value; }

		public StreamAudioInput(Stream sourceStream, WaveFormat format, IAudioData source) : base(source)
		{
			if (!sourceStream.CanRead)
				throw new Exception("Source stream must be readable!");

			SourceStream = sourceStream;
			WaveFormat = format;
		}

		public override int Read(byte[] buffer, int offset, int count) => SourceStream.Read(buffer, offset, count);
	}
}