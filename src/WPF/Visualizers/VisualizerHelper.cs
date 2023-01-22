using System;
using NAudio.Dsp;

namespace RCAudioPlayer.WPF.Visualizers
{
	public static class VisualizerHelper
	{
		private static float GetPower(Complex complex)
		{
			return MathF.Sqrt(complex.X * complex.X + complex.Y * complex.Y);
		}

		public static float[,] Split(ArraySegment<float> samples, int channels)
		{
			var channelSamples = new float[channels, samples.Count / channels];
			var samplesLength = samples.Count;
			var divSamplesLength = channelSamples.GetLength(1);
			for (int i = 0; i < samplesLength && i / channels < divSamplesLength; i++)
				channelSamples[i % channels, i / channels] = samples[i];
			return channelSamples;
		}

		public static float[,] FFT(ArraySegment<float> samples, int channels)
		{
			int channelLength = samples.Count / channels;
			int power = (int)Math.Log(channelLength, 2);
			int fftLength = (int)Math.Pow(2, power);
			var channelRates = new float[channels, fftLength];

			for (int c = 0; c < channels; c++)
			{
				var complices = new Complex[fftLength];
				for (int i = 0; i < complices.Length; i++)
					complices[i] = new Complex { X = samples[i * channels + c] };
				FastFourierTransform.FFT(true, power, complices);
				for (int i = 0; i < complices.Length; i++)
					channelRates[c, i] = GetPower(complices[i]);
			}

			return channelRates;
		}

		public static float[,] GroupCompress(float[,] channelData, int count, int length = -1)
		{
			int channels = channelData.GetLength(0);
			if (length == -1)
				length = channelData.GetLength(1);
			if (count == 0)
				count = 1;
			
			float[,] result = new float[channels, (int)MathF.Ceiling((float)length / count)];
			for (int c = 0; c < channels; c++)
				for (int i = 0; i < length; i++)
					if (channelData[c, i] > 0)
						result[c, i / count] = Math.Max(result[c, i / count], channelData[c, i]);
					else
						result[c, i / count] = Math.Min(result[c, i / count], channelData[c, i]);
			return result;
		}

		public static float[,] CountCompress(float[,] channelData, int count, int length = -1)
		{
			if (length == -1)
				length = channelData.GetLength(1);
			var groupSize = (int)Math.Floor(length / (float)count);
			return GroupCompress(channelData, groupSize, length);
		}

		public static float[,] Take(float[,] channelData, int length)
		{
			int channels = channelData.GetLength(0);
			if (length > channelData.GetLength(1))
				throw new ArgumentException("Target length is more than actual array's 2nd dimension's length!", nameof(length));
			float[,] result = new float[channels, length];
			for (int c = 0; c < channels; c++)
				for (int i = 0; i < length; i++)
					result[c, i] = channelData[c, i];
			return result;
		}
	}
}