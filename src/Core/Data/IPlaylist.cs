using System.Collections.Generic;

namespace RCAudioPlayer.Core.Data
{
	// Describes list of audio datas
	public interface IPlaylist
	{
		public List<IAudioData> List { get; }
	}

	// More precise variant of upper class
	public interface IPlaylist<E> : IPlaylist
		where E : IAudioData
	{
		public new List<E> List { get; }
	}
}