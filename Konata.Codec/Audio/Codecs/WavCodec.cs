using System.IO;
using System.Text;
using Konata.Codec.Utils;

namespace Konata.Codec.Audio.Codecs
{
    /// <summary>
    /// Wav codec
    /// </summary>
    public static class WavCodec
    {
        /// <summary>
        /// Wav encoder
        /// </summary>
        public class Encoder : AudioStream
        {
            private AudioInfo _output;
            private bool _firstRead;
            private bool _firstWrite;

            /// <summary>
            /// Wav encoder
            /// </summary>
            public Encoder()
            {
                _output = AudioInfo.Default();
                _firstRead = true;
                _firstWrite = true;
            }

            /// <summary>
            /// Wav encoder
            /// </summary>
            public Encoder(AudioInfo input) : this()
            {
                _output = input;
            }

            /// <summary>
            /// Set adaptive output
            /// </summary>
            internal override void SetAdaptiveInput(AudioInfo info)
                => _output = info;

            /// <inheritdoc />
            public override void Write(byte[] buffer, int offset, int count)
            {
                // Write wav header
                if (_firstWrite)
                {
                    _firstWrite = false;

                    using var writer = new BinaryWriter
                        (this, Encoding.Default, true);
                    {
                        writer.Write(1179011410U);
                        writer.Write(Length + 44 - 8);

                        writer.Write(1163280727U);
                        writer.Write(544501094U);

                        writer.Write(16);
                        writer.Write((ushort) 0x01);
                        writer.Write((ushort) _output.Channels);

                        writer.Write(_output.SampleRate);
                        writer.Write((uint) ((double) _output.SampleRate
                            * Sample.GetSampleLen(_output.Format) * (int) _output.Channels / 8));

                        writer.Write((ushort) Sample.GetSampleLen(_output.Format));

                        writer.Write(1635017060U);
                        writer.Write(Length);
                    }
                }

                base.Write(buffer, offset, count);
            }

            /// <inheritdoc />
            public override int Read(byte[] buffer, int offset, int count)
            {
                if (_firstRead)
                {
                    Position = 0;
                    _firstRead = false;
                }

                return base.Read(buffer, offset, count);
            }
        }
    }
}
