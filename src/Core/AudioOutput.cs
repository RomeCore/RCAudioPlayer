using System;
using NAudio.Wave;
using RCAudioPlayer.Core.Streams;

namespace RCAudioPlayer.Core
{
    public class AudioOutput : IDisposable
	{
		private WaveOut? _output;
		private PipelineStream? _provider;
		private float _volume;
		private bool _stoppedByFunc;

		public Action StoppedFunc;
		public Action StoppedNaturally;
		public event EventHandler Stopped = (s, e) => { };
		public event EventHandler Paused = (s, e) => { };
		public event EventHandler Resumed = (s, e) => { };

		public const int DefaultSampleRate = 44100;

		public PipelineStream? Pipeline { get => _provider; set { if (value != null) Init(value); } }
		public PlaybackState PlaybackState
		{
			get => _output?.PlaybackState ?? PlaybackState.Stopped; set
			{
				if (PlaybackState != value)
					switch (value)
					{
						case PlaybackState.Stopped:
							Stop();
							break;
						case PlaybackState.Paused:
							Pause();
							break;
						case PlaybackState.Playing:
							Play();
							break;
					}
			}
		}
		public float Volume { get => _output?.Volume ?? _volume; set {
				if (_output != null)
					_output.Volume = _volume = value;
				else
					_volume = value;
			}
		}
		
		public AudioOutput()
		{
			StoppedFunc = () => { };
			StoppedNaturally = () => { };
		}

		public void Dispose()
		{
			_output?.Dispose();
		}

		public void Init(PipelineStream provider)
		{
			float volume = Volume;
			_output?.Dispose();
			_output = new WaveOut();

			_output.Volume = volume;
			_output.PlaybackStopped += (s, e) =>
			{
				Stopped(this, EventArgs.Empty);
				StoppedFunc();
				if (!_stoppedByFunc)
					StoppedNaturally();
				_stoppedByFunc = false;
			};

			_provider = provider;
			_output.Init(_provider);
		}

		public void Play()
		{
			if (_output != null && PlaybackState == PlaybackState.Paused)
				Resumed(this, EventArgs.Empty);
			_output?.Play();
		}

		public void Pause()
		{
			if (_output != null && PlaybackState == PlaybackState.Playing)
				Paused(this, EventArgs.Empty);
			_output?.Pause();
		}

		public void PlayPause()
		{
			if (PlaybackState == PlaybackState.Stopped || PlaybackState == PlaybackState.Paused)
				Play();
			else
				Pause();
		}

		public void Stop()
		{
			if (_output != null)
			{
				if (PlaybackState == PlaybackState.Playing || PlaybackState == PlaybackState.Paused)
					Stopped(this, EventArgs.Empty);
				_stoppedByFunc = true;
				_output.Stop();
			}
		}
	}
}