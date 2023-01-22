using System.Collections.Generic;

namespace RCAudioPlayer.Core.Data
{
	public class Playlist<E> : IPlaylist<E>
		where E : class, IAudioData
	{
		List<IAudioData> IPlaylist.List => List.ConvertAll<IAudioData>(s => s);
		public List<E> List { get; } = new List<E>();
	}
}