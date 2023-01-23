using System;
using System.IO;
using NAudio.Wave;
using RCAudioPlayer.Core.Data;

namespace RCAudioPlayer.Core.Streams
{
	public abstract class AudioInput : WaveStream
	{
		public IAudioData Source { get; }

		public TimeSpan TSPosition
		{
			get => SampleConvert.TimeBytes(Position, WaveFormat);
			set => Position = SampleConvert.TimeBytes(value, WaveFormat);
		}
		public TimeSpan TSLength => SampleConvert.TimeBytes(Length, WaveFormat);

		public long SamplesPosition
		{
			get => Position / WaveFormat.BitsPerSample * 8;
			set => Position = value * WaveFormat.BitsPerSample / 8;
		}
		public long SamplesLength => Length / WaveFormat.BitsPerSample * 8;

		public AudioInput(IAudioData source)
		{
			Source = source;
		}

		public void WriteWave(Stream target)
		{
			var writer = new WaveFileWriter(target, WaveFormat);
			byte[] buffer = new byte[4096 * 16];
			Position = 0;
			while (true)
			{
				int read = Read(buffer, 0, buffer.Length);
				if (read == 0)
					break;
				writer.Write(buffer, 0, read);
			}
			writer.Close();
		}

		public static AudioInputStreamSource FromStream(WaveStream waveStream, IAudioData data)
		{
			return new AudioInputStreamSource(waveStream, data);
		}

		public static AudioInputStreamSource<T> FromStream<T>(T waveStream, IAudioData data) where T : WaveStream
		{
			return new AudioInputStreamSource<T>(waveStream, data);
		}
	}

	public class AudioInputStreamSource : AudioInput
	{
		public WaveStream WaveSource { get; }

		public override WaveFormat WaveFormat => WaveSource.WaveFormat;
		public override long Length => WaveSource.Length;
		public override long Position { get => WaveSource.Position; set => WaveSource.Position = value; }
		public override int Read(byte[] buffer, int offset, int count) => WaveSource.Read(buffer, offset, count);

		public AudioInputStreamSource(WaveStream waveSource, IAudioData source) : base(source)
		{
			WaveSource = waveSource;
		}
	}

	public class AudioInputStreamSource<T> : AudioInputStreamSource
		where T : WaveStream
	{
		public new T WaveSource => (T)base.WaveSource;

		public AudioInputStreamSource(T waveSource, IAudioData source) : base(waveSource, source)
		{
		}
	}
}