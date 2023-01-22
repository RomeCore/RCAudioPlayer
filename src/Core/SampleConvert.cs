using System;
using System.Linq;
using System.Runtime.InteropServices;
using NAudio.Wave;

namespace RCAudioPlayer.Core
{
	public static class SampleConvert
	{
		public static float ToFloat(short value) => value / (float)short.MaxValue;
		public static float ToFloat(int value) => value / (float)int.MaxValue;

		public static float[] ToFloat(byte[] buffer, int offset, int count)
        {
			return MemoryMarshal.Cast<byte, float>(buffer.AsSpan().Slice(offset, count)).ToArray();
		}

		public static float[] ToFloat(byte[] buffer, int offset, int count, WaveFormat inFormat)
        {
			bool isInloat = inFormat.Encoding == WaveFormatEncoding.IeeeFloat;
			int bytesPerSample = inFormat.BitsPerSample / 8;
			int size = count / bytesPerSample;
			float[] result = new float[size];
			
			if (bytesPerSample == 4)
			{
				if (isInloat)
					return MemoryMarshal.Cast<byte, float>(buffer.AsSpan().Slice(offset, count)).ToArray();
				else
					return MemoryMarshal.Cast<byte, int>(buffer.AsSpan().Slice(offset, count))
						.ToArray().Select(s => ToFloat(s)).ToArray();
			}
			else if (bytesPerSample == 2)
				return MemoryMarshal.Cast<byte, short>(buffer.AsSpan().Slice(offset, count))
					.ToArray().Select(s => ToFloat(s)).ToArray();
			throw new NotSupportedException($"Wave format is not supported: {inFormat}");
		}

		public static float[] ToFloat(IWaveProvider provider, byte[] buffer, int offset, int count, WaveFormat inFormat)
        {
			count = count * inFormat.BitsPerSample / 32;
			return ToFloat(buffer, offset, provider.Read(buffer, offset, count), inFormat);
        }

		public static float[] ToFloat(IWaveProvider provider, byte[] buffer, int offset, int count)
        {
			return ToFloat(buffer, offset, provider.Read(buffer, offset, count));
		}

		public static int ToBuffer(ReadOnlySpan<float> samples, byte[] buffer, int offset)
        {
			int count = samples.Length * sizeof(float);
			Buffer.BlockCopy(samples.ToArray(), 0, buffer, offset, count);
			return count;
        }

        #region Time

        public static TimeSpan TimeBytes(long value, WaveFormat format)
        {
            return TimeSpan.FromSeconds((double)value / format.SampleRate / format.Channels / format.BitsPerSample * 8);
        }

        public static TimeSpan TimeSamples(long value, WaveFormat format)
        {
            return TimeSpan.FromSeconds((double)value / format.SampleRate / format.Channels);
        }

        public static long TimeBytes(TimeSpan timeSpan, WaveFormat format)
        {
            return (long)(timeSpan.TotalSeconds * format.SampleRate * format.Channels * format.BitsPerSample / 8);
        }

        public static long TimeSamples(TimeSpan timeSpan, WaveFormat format)
		{
			return (long)(timeSpan.TotalSeconds * format.SampleRate * format.Channels);
		}
		
        public static double SecondsBytes(long value, WaveFormat format)
        {
            return SecondsBytes((double)value, format);
        }

        public static double SecondsSamples(long value, WaveFormat format)
        {
            return SecondsSamples((double)value, format);
        }

        public static long SecondsBytes(double seconds, WaveFormat format)
        {
            return (long)(seconds * format.SampleRate * format.Channels * format.BitsPerSample / 8);
        }

        public static long SecondsSamples(double seconds, WaveFormat format)
		{
			return (long)(seconds * format.SampleRate * format.Channels);
		}

        #endregion
    }
}