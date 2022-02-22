using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Konata.Codec.Exceptions;

// ReSharper disable InvertIf
// ReSharper disable AccessToDisposedClosure

namespace Konata.Codec.Audio.Codecs
{
    /// <summary>
    /// Silk codec
    /// </summary>
    public static class SilkV3Codec
    {
        [DllImport("SilkCodec", EntryPoint = "silkEncode")]
        private static extern bool SilkEncode(IntPtr pcmData, int dataLen,
            int sampleRate, CodecCallback cb, IntPtr userData);

        [DllImport("SilkCodec", EntryPoint = "silkDecode")]
        private static extern bool SilkDecode(IntPtr silkData, int dataLen,
            int sampleRate, CodecCallback cb, IntPtr userData);

        private delegate void CodecCallback(IntPtr userData, IntPtr p, int len);

        /// <summary>
        /// SilkCodec encoder
        /// </summary>
        public class Encoder : AudioStream
        {
            private bool _firstRead;

            /// <summary>
            /// SilkCodec encoder
            /// </summary>
            public Encoder()
            {
                _firstRead = true;
            }

            /// <inheritdoc />
            public override int Read(byte[] buffer, int offset, int count)
            {
                // Encode
                if (_firstRead)
                {
                    _firstRead = false;

                    // Duplicate the data
                    var pcmData = ToArray();
                    var lpPcmData = Marshal.AllocHGlobal(pcmData.Length);

                    // Copy the data
                    Marshal.Copy(pcmData, 0, lpPcmData, pcmData.Length);

                    // Cleanup the stream
                    SetLength(0);
                    Position = 0;

                    try
                    {
                        // Prepare the stream
                        using var binaryWriter = new BinaryWriter
                            (this, Encoding.Default, true);

                        // Encode the pcm data
                        var result = SilkEncode(lpPcmData,
                            pcmData.Length, 24000, (_, data, length) =>
                            {
                                // Copy the part
                                var outbuf = new byte[length];
                                Marshal.Copy(data, outbuf, 0, length);

                                // Write to stream
                                binaryWriter.Write(outbuf);
                            }, IntPtr.Zero);

                        // Failed
                        if (!result) throw new EncodeException("");

                        // Move position
                        Position = 0;
                    }

                    // Catch native exceptions
                    catch
                    {
                        throw new EncodeException("Thrown an exception while encoding silk.");
                    }

                    // Cleanup
                    finally
                    {
                        Marshal.FreeHGlobal(lpPcmData);
                    }
                }

                return base.Read(buffer, offset, count);
            }
        }

        /// <summary>
        /// SilkCodec encoder
        /// </summary>
        public class Decoder : AudioStream
        {
            private bool _firstRead;

            /// <summary>
            /// SilkCodec encoder
            /// </summary>
            public Decoder()
            {
                _firstRead = true;
            }

            /// <summary>
            /// Get adaptive output
            /// </summary>
            internal override AudioInfo? GetAdaptiveOutput()
                => AudioInfo.SilkV3();

            /// <inheritdoc />
            public override int Read(byte[] buffer, int offset, int count)
            {
                // Encode
                if (_firstRead)
                {
                    _firstRead = false;

                    // Duplicate the data
                    var silkData = ToArray();
                    var lpSilkData = Marshal.AllocHGlobal(silkData.Length);

                    // Copy the data
                    Marshal.Copy(silkData, 0, lpSilkData, silkData.Length);

                    // Cleanup the stream
                    SetLength(0);
                    Position = 0;

                    try
                    {
                        // Prepare the stream
                        using var binaryWriter = new BinaryWriter
                            (this, Encoding.Default, true);

                        // Decode the silk data
                        var result = SilkDecode(lpSilkData,
                            silkData.Length, 24000, (_, data, length) =>
                            {
                                // Copy the part
                                var outbuf = new byte[length];
                                Marshal.Copy(data, outbuf, 0, length);

                                // Write to stream
                                binaryWriter.Write(outbuf);
                            }, IntPtr.Zero);

                        // Failed
                        if (!result) throw new EncodeException("");

                        // Move position
                        Position = 0;
                    }

                    // Catch native exceptions
                    catch
                    {
                        throw new EncodeException("Thrown an exception while decoding silk.");
                    }

                    // Cleanup
                    finally
                    {
                        Marshal.FreeHGlobal(lpSilkData);
                    }
                }

                return base.Read(buffer, offset, count);
            }
        }
    }
}
