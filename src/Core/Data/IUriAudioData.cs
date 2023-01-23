namespace RCAudioPlayer.Core.Data
{
	// Audio data that uses uri for data extraction
	// Most common variant of getting data
	// !!! If you have fully ready class that uses this interface, it MUST have to be in type dictionary:
	//    Constructor with (string uri)
	//    UriSupports attribute to specify extensions that it supports
	public interface IUriAudioData : IAudioData
	{
		public string Uri { get; }
	}
}