using System;
using System.Threading.Tasks;
using RCAudioPlayer.Core.Streams;

namespace RCAudioPlayer.Core.Playables
{
	public class ErrorPlayable : IPlayable
	{
		private string _str;

		public string String => _str;
		public Exception? Exception { get; }
		public string FullTitle => _str;

		public ErrorPlayable(string str) : this(str, null)
		{
		}
		
		public ErrorPlayable(string str, Exception? exc)
		{
			_str = str;
			Exception = exc;
		}

		public string Save()
		{
			return _str;
		}

		public bool SkipBackward()
		{
			return true;
		}

		public bool SkipForward()
		{
			return true;
		}

		public Task<AudioInput> GetInput()
		{
			return Task.FromResult<AudioInput>(new EmptyAudioInput());
		}
	}
}