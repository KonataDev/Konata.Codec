using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

// ReSharper disable InvertIf
// ReSharper disable NotAccessedField.Local
// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
// ReSharper disable ConvertSwitchStatementToSwitchExpression

namespace Konata.Codec
{
    /// <summary>
    /// Audio bit format
    /// </summary>
    public enum Format
    {
        /// <summary>
        /// Signed 16 bit
        /// </summary>
        Signed16Bit,

        /// <summary>
        /// Signed 32 bit
        /// </summary>
        Signed32Bit,

        /// <summary>
        /// Unsigned 8 bit
        /// </summary>
        UnSigned8Bit,

        /// <summary>
        /// Float 32 bit
        /// </summary>
        Float32Bit,

        /// <summary>
        /// Float 64 bit
        /// </summary>
        Float64Bit
    }

    /// <summary>
    /// Simple audio resampler
    /// </summary>
    public class AudioResampler
    {
        private readonly byte[] _fromData;

        // Conversion configs
        private (int Rate, int Channels, Format Format) _toConfig;
        private (int Rate, int Channels, Format Format) _fromConfig;

        /// <summary>
        /// Audio sampler
        /// </summary>
        /// <param name="fromData"></param>
        public AudioResampler(byte[] fromData)
        {
            _fromData = fromData;
            _toConfig = (44100, 2, Format.Float64Bit);
            _fromConfig = (44100, 2, Format.Signed16Bit);
        }

        /// <summary>
        /// Set target format
        /// </summary>
        /// <param name="sampleRate"><b>[In]</b> Sample rate. default is 44.1Khz</param>
        /// <param name="channel"><b>[In]</b> Channel number. default is 2 (stereo)</param>
        /// <param name="format"><b>[In]</b> Format. default is Format.Float64Bit</param>
        public void SetTarget(int sampleRate, int channel, Format format)
        {
            _toConfig.Rate = sampleRate;
            _toConfig.Format = format;
            _toConfig.Channels = channel;
        }

        /// <summary>
        /// Set original format
        /// </summary>
        /// <param name="sampleRate"><b>[In]</b> Sample rate. default is 44.1Khz</param>
        /// <param name="channel"><b>[In]</b> Channel number. default is 2 (stereo)</param>
        /// <param name="format"><b>[In]</b> Format. default is Format.Signed16Bit</param>
        public void SetOrigin(int sampleRate, int channel, Format format)
        {
            _fromConfig.Rate = sampleRate;
            _fromConfig.Format = format;
            _fromConfig.Channels = channel;
        }

        /// <summary>
        /// Start the convert
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<bool> Resample(out byte[] data)
        {
            data = null;

            // No need to convert
            if (_fromConfig.Rate == _toConfig.Rate
                && _fromConfig.Format == _toConfig.Format
                && _fromConfig.Channels == _toConfig.Channels)
            {
                data = _fromData;
                return Task.FromResult(true);
            }

            // Calculate
            var fromChannels = _fromConfig.Channels;
            var fromSampleLen = GetSampleLen(_fromConfig.Format);
            var fromChnSamples = _fromData.Length / fromSampleLen / fromChannels;

            var toChannels = _toConfig.Channels;
            var toChnFactor = (double) _fromConfig.Rate / _toConfig.Rate;
            var toChnSamples = (long) ((double) _toConfig.Rate / _fromConfig.Rate * fromChnSamples);

            // Prepare channel space
            var toChnData = new List<double[]>();
            {
                for (var i = 0; i < toChannels; ++i)
                    toChnData.Add(new double[toChnSamples]);
            }

            // Start convert
            using var inputStream = new MemoryStream(_fromData);
            using var inputReader = new BinaryReader(inputStream);
            {
                for (var i = 0; i < toChannels; ++i)
                {
                    // Select the channel
                    var channel = i < fromChannels
                        ? i
                        : i % fromChannels;

                    // Resample the data
                    for (long j = 0; j < toChnSamples; ++j)
                    {
                        // Calculate the sample scale
                        var sampleScale = toChnFactor * j;
                        {
                            // Align to the channel
                            sampleScale -= sampleScale % fromChannels;
                            sampleScale += channel;
                        }

                        // Seek to source sample
                        var samplePos = sampleScale * fromSampleLen * fromChannels;
                        inputStream.Seek((long) samplePos, SeekOrigin.Begin);
                        {
                            // Convert a sample
                            var sample = ReadSampleData(inputReader, _fromConfig.Format);
                            toChnData[i][j] = sample;
                        }
                    }
                }
            }

            // Save audio data
            using var outputStream = new MemoryStream();
            using var outputWriter = new BinaryWriter(outputStream);
            {
                for (var i = 0; i < toChnSamples; ++i)
                for (var j = 0; j < toChannels; ++j)
                    WriteSampleData(outputWriter, _toConfig.Format, toChnData[j][i]);
            }

            // Get data
            data = outputStream.ToArray();
            return Task.FromResult(data?.Length != 0);
        }

        /// <summary>
        /// Get format align
        /// </summary>
        /// <returns></returns>
        private static int GetSampleLen(Format format)
            => format switch
            {
                Format.UnSigned8Bit => 1,
                Format.Signed16Bit => 2,
                Format.Signed32Bit => 4,
                Format.Float32Bit => 4,
                Format.Float64Bit => 8,
                _ => 0
            };

        /// <summary>
        /// Read sample data
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        private static double ReadSampleData(BinaryReader reader, Format format)
        {
            switch (format)
            {
                case Format.UnSigned8Bit:
                    return (double) reader.ReadSByte() / sbyte.MaxValue;

                case Format.Signed16Bit:
                    return (double) reader.ReadInt16() / short.MaxValue;

                case Format.Signed32Bit:
                    return (double) reader.ReadInt32() / int.MaxValue;

                case Format.Float32Bit:
                    return (double) reader.ReadSingle() / float.MaxValue;

                case Format.Float64Bit:
                    return reader.ReadDouble();
            }

            return 0;
        }

        /// <summary>
        /// Write sample data
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="format"></param>
        /// <param name="data"></param>
        private static void WriteSampleData(BinaryWriter writer, Format format, double data)
        {
            switch (format)
            {
                case Format.UnSigned8Bit:
                    writer.Write((byte) (data * sbyte.MaxValue));
                    break;

                case Format.Signed16Bit:
                    writer.Write((short) (data * short.MaxValue));
                    break;

                case Format.Signed32Bit:
                    writer.Write((int) (data * int.MaxValue));
                    break;

                case Format.Float32Bit:
                    writer.Write((float) data);
                    break;

                case Format.Float64Bit:
                    writer.Write(data);
                    break;
            }
        }
    }
}
