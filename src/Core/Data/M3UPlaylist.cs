using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RCAudioPlayer.Core.Streams;

namespace RCAudioPlayer.Core.Data
{
	[UriSupports("m3u", "m3u8")]
	public class M3UPlaylist : Playlist<M3UElement>, IAudioData
	{
		public string Uri { get; }
        public float Length { get; }
        public Dictionary<string, object> Data { get; }

		public M3UPlaylist(string uri, Dictionary<string, object>? data = null)
		{
			Uri = uri;
            Data = data ?? new Dictionary<string, object>();

            var fileData = NetClient.GetString(uri);
			var currentData = new Dictionary<string, object>();
			foreach (var line in fileData.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
				if (!string.IsNullOrEmpty(line))
				{
					if (!line.StartsWith('#'))
					{
						var _uri = uri;
						if (File.Exists(line))
							uri = line;
						else
						{
							if (uri.Contains("index.m3u8"))
                                _uri = uri.Replace("index.m3u8", line);
							else if (uri.Contains("index.m3u"))
                                _uri = uri.Replace("index.m3u", line);
							else
                                _uri = Path.Combine(uri, line);
						}
						List.Add(new M3UElement(_uri, currentData.ToDictionary(k => k.Key, s => s.Value)));
						currentData.Clear();
                    }
					else
                    {
                        var _line = line.Substring(line.IndexOf(':') + 1);
						var entries = _line.Split(",", StringSplitOptions.RemoveEmptyEntries);
                        if (line.StartsWith("#EXTINF"))
						{
							currentData["length"] = float.Parse(entries[0].Replace('.', ','));
							if (entries.Length > 1)
							{
								var infos = entries[1].Split(" - ", 2);
								if (infos.Length > 1)
								{
									currentData["artist"] = infos[0];
									currentData["title"] = infos[1];
								}
								else
									currentData["title"] = entries[1];
                            }
                        }
						else if (line.StartsWith("#EXT-X-KEY"))
						{
							foreach (var entry in entries)
							{
								var keyValue = entry.Split('=', 2);
								var key = keyValue[0].ToLower();
								var value = keyValue[1];
								currentData[key] = value.Trim('"');
							}
						}
					}
				}

            Length = List.Select(s => s.Length).Sum();
        }

		public AudioInput GetInput()
		{
			return AudioInput.FromStream(
				new MediaFoundationStream(
					GetRawInput()
					), this);
		} 

		public Stream GetRawInput()
		{
			return new CombinedStream(List.ConvertAll(s => s.GetRawInput()));
		}
	}
}