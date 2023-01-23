using System;
using System.IO;
using System.Collections.Generic;

namespace RCAudioPlayer.Core.Streams
{
	public class CombinedStream : Stream
	{
		bool _canRead;
		bool _canWrite;
		long _length;
		long _position;

		private IReadOnlyList<Stream> _sources;
		private Dictionary<long, Stream> _sourcesStarts;
		private Dictionary<long, Stream>.Enumerator _enumerator;

		private void UpdatePosition()
		{
			try
			{
				_enumerator.Dispose();
			}
			catch { }

			_enumerator = _sourcesStarts.GetEnumerator();
			_enumerator.MoveNext();
			var secondEnumerator = _sourcesStarts.GetEnumerator();
			while (_enumerator.Current.Key <= _position && secondEnumerator.Current.Key > Position)
			{
				secondEnumerator.MoveNext();
				if (!_enumerator.MoveNext())
					break;
			}
			_enumerator.Current.Value.Position = _position - _enumerator.Current.Key;
		}

		public IReadOnlyList<Stream> Sources { get => _sources; set
			{
				_sources = value;
				_sourcesStarts.Clear();

				_canRead = true;
				_canWrite = true;
				_length = 0;

				foreach (var source in _sources)
				{
					_sourcesStarts.Add(_length, source);

					if (!source.CanSeek)
						throw new NotSupportedException($"Stream must be seekable!: {source}");
					_canRead &= source.CanRead;
					_canWrite &= source.CanWrite;
					_length += source.Length;
				}

				if (_position > _length)
					_position = _length;
				UpdatePosition();
			}
		}

		public override bool CanSeek => true;
		public override bool CanRead => _canRead;
		public override bool CanWrite => _canWrite;
		public override long Length => _length;
		public override long Position { get => _position; set => Seek(value, SeekOrigin.Begin); }

		public CombinedStream(IReadOnlyList<Stream>? sources = null)
		{
			_sourcesStarts = new Dictionary<long, Stream>();
			_sources = Sources = sources ?? new List<Stream>().AsReadOnly();
		}

		public override void Flush()
		{
			foreach (var source in _sources)
				source.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			switch (origin)
			{
				case SeekOrigin.Begin:
					_position = offset;
					break;
				case SeekOrigin.Current:
					_position += offset;
					break;
				case SeekOrigin.End:
					_position = _length - offset - 1;
					break;
				default:
					_position = 0;
					break;
			}

			if (_position > _length)
				_position = _length;
			UpdatePosition();

			return _position;
		}

		public override void Close()
		{
			base.Close();
			foreach (var source in _sources)
				source.Close();
		}

		public override void SetLength(long value)
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (!_canRead)
				return 0;

			int read = 0;
			while (read < count)
			{
				int _read = 0;
				var current = _enumerator.Current.Value;
				try
				{
					current.Position = _position - _enumerator.Current.Key;
					_read = current.Read(buffer, offset + read, count - read);
					read += _read;
					_position += _read;
				}
				catch (EndOfStreamException) { }

				if (_read == 0)
				{
					current.Position = current.Length;
					if (!_enumerator.MoveNext())
						break;
				}
			}

			return read;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (!_canWrite)
				return;

			int written = 0;
			while (written < count)
			{
				int _written = 0;
				var current = _enumerator.Current.Value;
				try
				{
					long posBefore = current.Position = _position - _enumerator.Current.Key;
					current.Write(buffer, offset + written, count - written);
					_written = (int)(current.Position - posBefore);
					written += _written;
					_position += _written;
				}
				catch (EndOfStreamException) { }

				if (_written == 0)
				{
					current.Position = current.Length;
					if (!_enumerator.MoveNext())
						break;
				}
			}
		}
	}
}