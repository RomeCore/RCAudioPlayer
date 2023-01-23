using NAudio.Wave;

namespace RCAudioPlayer.Core.Streams
{
	public class EmptyAudioInput : AudioInput
	{
#pragma warning disable CS8625 // Литерал, равный NULL, не может быть преобразован в ссылочный тип, не допускающий значение NULL.
		public EmptyAudioInput() : base(null)
#pragma warning restore CS8625 // Литерал, равный NULL, не может быть преобразован в ссылочный тип, не допускающий значение NULL.
		{
		}

		public override WaveFormat WaveFormat => WaveFormat.CreateIeeeFloatWaveFormat(AudioOutput.DefaultSampleRate, 2);

		public override long Length => 0;

		public override long Position { get => 0; set => _ = value; }

		public override int Read(byte[] buffer, int offset, int count)
		{
			return 0;
		}
	}
}