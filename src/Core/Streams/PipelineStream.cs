using System;
using NAudio.Wave;
using System.Threading;
using System.IO;

namespace RCAudioPlayer.Core.Streams
{
	public class PipelineStream : WaveStream, IDisposable
	{
		public class PositionChangedArgs
		{
			public long OldPosition { get; } = -1;
			public long NewPosition { get; }
			public long OldLength { get; } = -1;
			public long NewLength { get; }

			public PositionChangedArgs(long oldPosition, long newPosition, long oldLength, long newLength)
			{
				OldPosition = oldPosition;
				NewPosition = newPosition;
				OldLength = oldLength;
				NewLength = newLength;
			}
		}

		public class SamplesReceivedArgs
		{
			public int SampleCount { get; }
			public float[] Samples { get; }
			public WaveFormat OutputFormat { get; }
			public TimeSpan ProcessingTime { get; }

			public SamplesReceivedArgs(float[] samples, WaveFormat outputFormat, TimeSpan processingTime)
			{
				SampleCount = samples.Length;
				Samples = samples;
				OutputFormat = outputFormat;
				ProcessingTime = processingTime;
			}
		}

		private readonly Mutex _inputLock;

		private long _prevPosition;
		private long _prevLength;
		private AudioInput? _inputStream;
		private readonly PipelineManager _pipelineManager;

		public AudioInput? InputStream
		{
			get => _inputStream; set
			{
				_inputLock.WaitOne();
				if (_inputStream != null)
					_inputStream.Dispose();
				_inputStream = value;
				_inputLock.ReleaseMutex();
			}
		}
		public PipelineManager PipelineManager => _pipelineManager;

		public override WaveFormat WaveFormat => _inputStream != null ?
			WaveFormat.CreateIeeeFloatWaveFormat(_inputStream.WaveFormat.SampleRate, _inputStream.WaveFormat.Channels) :
			WaveFormat.CreateIeeeFloatWaveFormat(AudioOutput.DefaultSampleRate, 2);
		public WaveFormat InputWaveFormat => _inputStream?.WaveFormat ?? throw new Exception("Input stream is null!");

		public override long Length => _inputStream != null ? _inputStream.Length : 0;
		public override long Position
		{
			get => _inputStream != null ? _inputStream.Position : 0;
			set
			{
				if (_inputStream != null && _inputStream.Position != value)
				{
					PositionChanged(this, new PositionChangedArgs(_inputStream.Position, value, Length, Length));
					_inputStream.Position = value;
				}
			}
		}

		public event EventHandler<PositionChangedArgs> PositionChanged = (s, e) => { };
		public event EventHandler<SamplesReceivedArgs>? SamplesReceived;

		public PipelineStream()
		{
			_inputLock = new Mutex();

			_prevPosition = 0;
			_pipelineManager = new PipelineManager();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			_inputLock.Dispose();
			_inputStream?.Dispose();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			PipelineManager.Current?.ClearState();
			return base.Seek(offset, origin);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			try
			{
				if (InputStream == null)
					throw new NullReferenceException($"{nameof(InputStream)} was null");

				var outputFormat = WaveFormat;
				var timeStart = DateTime.Now;

				PositionChanged(this, new PositionChangedArgs(_prevPosition, Position, _prevLength, Length));
				_prevPosition = Position;
				_prevLength = Length;

				var samples = _pipelineManager.ReceiveSamples(count / 4, InputStream, outputFormat);
				if (SamplesReceived != null)
					SamplesReceived(this, new SamplesReceivedArgs(samples.ToArray(), outputFormat, DateTime.Now - timeStart));
				var bytes = SampleConvert.ToBuffer(samples, buffer, offset);

				return bytes;
			}
			catch (Exception exc)
			{
				Log.Exception(exc, "while processing audio pipeline");
				return 0;
			}
		}
	}
}