using System;
using NAudio.Wave;
using Newtonsoft.Json.Linq;

namespace RCAudioPlayer.Core.Pipeline
{
	public class PipelineInputElement : PipelineElement
	{
		public override int AvailableSamples => 0;

		public WaveStream InputStream { get; }

		public PipelineInputElement(WaveStream inputStream)
		{
			InputStream = inputStream;
		}

		public override void Reset()
		{
		}

		public override void Deserialize(JContainer? obj)
		{
			throw new NotImplementedException();
		}

		public override JContainer Serialize()
		{
			throw new NotImplementedException();
		}

		public override void UpdateFormat(WaveFormat format)
		{
			throw new NotImplementedException();
		}

		protected override void Put(Span<float> samples)
		{
		}

		protected override Span<float> Get(int count)
		{
			return Receive(InputStream, count);
		}

		public static Span<float> Receive(WaveStream inputStream, int count)
		{
			var inputFormat = inputStream.WaveFormat;
			var buffer = new byte[count * inputFormat.BitsPerSample / 8];
			var readBytes = inputStream.Read(buffer, 0, buffer.Length);
			return SampleConvert.ToFloat(buffer, 0, readBytes, inputFormat);
		}

		public override Span<float> Receive(int count)
		{
			return Get(count);
		}
	}
}