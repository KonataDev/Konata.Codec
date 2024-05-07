using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NVorbis;

namespace Konata.Codec.Audio.Codecs;

/// <summary>
/// Vorbis codec
/// </summary>
public class VorbisCodec
{
    /// <summary>
    /// Vorbis decoder
    /// </summary>
    public class Decoder : AudioStream
    {
        private readonly VorbisReader _stream;
        private readonly AudioInfo _output;

        /// <summary>
        /// Vorbis decoder
        /// </summary>
        public Decoder(string file)
        {
            _stream = new VorbisReader(file);
            _output = new AudioInfo(AudioFormat.Float32Bit,
                (AudioChannel) _stream.Channels, _stream.SampleRate);
        }

        /// <summary>
        /// Vorbis decoder
        /// </summary>
        public Decoder(Stream file)
        {
            _stream = new VorbisReader(file);
        }

        /// <summary>
        /// Get adaptive output
        /// </summary>
        internal override AudioInfo? GetAdaptiveOutput()
            => _output;

        /// <inheritdoc />
        public override unsafe int Read(byte[] buffer, int offset, int count)
        {
            var len = count / sizeof(float);
            var readbuf = new float[len];
            len = _stream.ReadSamples(readbuf, offset, len) * sizeof(float);

            fixed (float* floatPtr = readbuf)
            {
                fixed (byte* bytePtr = buffer)
                {
                    var src = (byte*) floatPtr;
                    var dst = bytePtr;

                    for (var i = offset; i < count; i++)
                    {
                        *dst = *src;
                        src++;
                        dst++;
                    }
                }
            }

            return len;
        }

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin)
        {
            _stream.SeekTo(offset, origin);
            return _stream.SamplePosition;
        }
    }
}
