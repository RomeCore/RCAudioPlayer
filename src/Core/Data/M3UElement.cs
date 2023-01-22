using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using RCAudioPlayer.Core.Streams;

namespace RCAudioPlayer.Core.Data
{
	public class M3UElement : IAudioData
	{
		public string Uri { get; }
		public float Length { get; }
		public Dictionary<string, object> Data { get; }

		public string? Artist { get; }
		public string? Title { get; }

		public M3UElement(string uri, Dictionary<string, object>? data = null)
		{
			Uri = uri;
			Data = data ?? new Dictionary<string, object>();

			Length = Data.Get("length") as float? ?? 0.0f;
			Artist = Data.Get("artist") as string;
			Title = Data.Get("title") as string;
		}

		public AudioInput GetInput()
		{
			if (Data.Get("method") as string != null)
			{
				var stream = GetRawInput();
				return AudioInput.FromStream(new MediaFoundationStream(stream), this);
			}

			var audioData = AudioDictionary.Get(Uri);
			if (audioData != null)
				return audioData.GetInput();
			throw new Exception($"Can't find correct stream for this uri: {Uri}");
		}

		public Stream GetRawInput()
		{
			var byteData = NetClient.Get(Uri);

			if (Data.Get("method") is string cipherMethod)
			{
				SymmetricAlgorithm? alg = null;
				switch (cipherMethod)
				{
					case "AES-128":
						alg = Aes.Create();
						alg.Mode = CipherMode.CBC;
						alg.KeySize = 128;
						break;
				}

				if (alg != null)
				{
					var keyUri = Data.Get("uri") as string ?? "";
					var key = Data.Get("key") is string _key ? Encoding.UTF8.GetBytes(_key)
						: NetClient.Get(keyUri);
					var _iv = Data.Get("iv") as string;
					byte[] iv = _iv != null ? Encoding.UTF8.GetBytes(_iv) : new byte[key.Length];
					if (_iv == null)
						Array.Copy(byteData, iv, iv.Length);
					int _offset = _iv == null ? iv.Length : 0;

					alg.Key = key;
					alg.IV = iv;

					var length = byteData.Length - _offset;
					var cryptoStream = new CryptoStream(new MemoryStream(byteData, _offset, length),
						alg.CreateDecryptor(), CryptoStreamMode.Read);
					byte[] decryptedData = new byte[length];

					int offset = 0;
					int blockSize = 4096 * 4;
					while (true)
					{
						try
						{
							var read = cryptoStream.Read(decryptedData, offset, Math.Min(blockSize, length - offset));
							if (read == 0)
								break;
							offset += read;
						} catch (EndOfStreamException) { break; }
					}
					cryptoStream.Close();
					byteData = decryptedData;
				}
			}
			return new MemoryStream(byteData);
		}
	}
}