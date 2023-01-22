using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using NAudio.MediaFoundation;
using NAudio.Utils;
using NAudio.Wave;

namespace RCAudioPlayer.Core.Streams
{
    public class MediaFoundationStream : MediaFoundationReader
    {
        private class ComStream : Stream, IStream
        {
            private readonly Stream _source;

            public override bool CanRead => _source.CanRead;

            public override bool CanSeek => _source.CanSeek;

            public override bool CanWrite => _source.CanWrite;

            public override long Length => _source.Length;

            public override long Position
            {
                get { return _source.Position; }
                set { _source.Position = value; }
            }

            public ComStream(Stream stream)
                : this(stream, false)
            {
            }

            internal ComStream(Stream stream, bool synchronizeStream)
            {
                if (stream == null)
                    throw new ArgumentNullException(nameof(stream));
                if (synchronizeStream)
                    stream = Synchronized(stream);
                this._source = stream;
            }

            void IStream.Clone(out IStream ppstm)
            {
#pragma warning disable CS8625
                ppstm = null;
#pragma warning restore CS8625
            }

            void IStream.Commit(int grfCommitFlags)
            {
                _source.Flush();
            }

            void IStream.CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
            {
            }

            void IStream.LockRegion(long libOffset, long cb, int dwLockType)
            {
            }

            void IStream.Read(byte[] pv, int cb, IntPtr pcbRead)
            {
                if (!CanRead)
                    throw new InvalidOperationException("Stream is not readable.");
                int val = Read(pv, 0, cb);
                if (pcbRead != IntPtr.Zero)
                    Marshal.WriteInt32(pcbRead, val);
            }

            void IStream.Revert()
            {
            }

            void IStream.Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
            {
                SeekOrigin origin = (SeekOrigin)dwOrigin;
                long val = Seek(dlibMove, origin);
                if (plibNewPosition != IntPtr.Zero)
                    Marshal.WriteInt64(plibNewPosition, val);
            }

            void IStream.SetSize(long libNewSize)
            {
                SetLength(libNewSize);
            }

            void IStream.Stat(out STATSTG pstatstg, int grfStatFlag)
            {
                const int STGM_READ = 0x00000000;
                const int STGM_WRITE = 0x00000001;
                const int STGM_READWRITE = 0x00000002;

                var tmp = new STATSTG { type = 2, cbSize = Length, grfMode = 0 };

                if (CanWrite && CanRead)
                    tmp.grfMode |= STGM_READWRITE;
                else if (CanRead)
                    tmp.grfMode |= STGM_READ;
                else if (CanWrite)
                    tmp.grfMode |= STGM_WRITE;
                else
                    throw new ObjectDisposedException("Stream");

                pstatstg = tmp;
            }

            void IStream.UnlockRegion(long libOffset, long cb, int dwLockType)
            {
            }

            void IStream.Write(byte[] pv, int cb, IntPtr pcbWritten)
            {
                if (!CanWrite)
                    throw new InvalidOperationException("Stream is not writeable.");
                Write(pv, 0, cb);
                if (pcbWritten != IntPtr.Zero)
                    Marshal.WriteInt32(pcbWritten, cb);
            }

            public override void Flush()
            {
                _source.Flush();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return _source.Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return _source.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                _source.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                _source.Write(buffer, offset, count);
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if (_source == null)
                    return;
                _source.Dispose();
            }

            public override void Close()
            {
                base.Close();
                if (_source == null)
                    return;
                _source.Close();
            }
        }

        private Stream _sourceStream;

        public MediaFoundationStream(Stream sourceStream)
        {
            _sourceStream = sourceStream;
            Init(null);
        }

        public MediaFoundationStream(Stream sourceStream, MediaFoundationReaderSettings settings)
        {
            _sourceStream = sourceStream;
            Init(settings);
        }

        private static MediaType GetCurrentMediaType(IMFSourceReader reader)
        {
            reader.GetCurrentMediaType(-3, out var ppMediaType);
            return new MediaType(ppMediaType);
        }

        protected override IMFSourceReader CreateReader(MediaFoundationReaderSettings settings)
        {
            MediaFoundationInterop.MFCreateMFByteStreamOnStream(new ComStream(_sourceStream), out var byteStream);
            MediaFoundationInterop.MFCreateSourceReaderFromByteStream(byteStream, null, out var ppSourceReader);

            ppSourceReader.SetStreamSelection(-2, pSelected: false);
            ppSourceReader.SetStreamSelection(-3, pSelected: true);
            MediaType mediaType = new MediaType();
            mediaType.MajorType = MediaTypes.MFMediaType_Audio;
            mediaType.SubType = settings.RequestFloatOutput ? AudioSubtypes.MFAudioFormat_Float : AudioSubtypes.MFAudioFormat_PCM;
            MediaType currentMediaType = GetCurrentMediaType(ppSourceReader);
            mediaType.ChannelCount = currentMediaType.ChannelCount;
            mediaType.SampleRate = currentMediaType.SampleRate;
            try
            {
                ppSourceReader.SetCurrentMediaType(-3, IntPtr.Zero, mediaType.MediaFoundationObject);
            }
            catch (COMException exception) when (exception.GetHResult() == -1072875852)
            {
                if (!(currentMediaType.SubType == AudioSubtypes.MFAudioFormat_AAC) || currentMediaType.ChannelCount != 1)
                {
                    throw;
                }

                mediaType.SampleRate = currentMediaType.SampleRate *= 2;
                mediaType.ChannelCount = currentMediaType.ChannelCount *= 2;
                ppSourceReader.SetCurrentMediaType(-3, IntPtr.Zero, mediaType.MediaFoundationObject);
            }

            Marshal.ReleaseComObject(currentMediaType.MediaFoundationObject);
            return ppSourceReader;
        }
    }
}
