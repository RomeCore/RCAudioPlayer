using System.Collections.Generic;

namespace RCAudioPlayer.Core.Data
{
	public interface IPlaylist
	{
		public List<IAudioData> List { get; }
	}

	public interface IPlaylist<E> : IPlaylist
		where E : IAudioData
	{
		public new List<E> List { get; }
    }
}