using Konata.Codec.Utils;

// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable InvertIf
// ReSharper disable NotAccessedField.Local
// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
// ReSharper disable ConvertSwitchStatementToSwitchExpression

namespace Konata.Codec.Audio
{
    /// <summary>
    /// Simple audio resampler
    /// </summary>
    public class AudioResampler : AudioStream
    {
        // Conversion configs
        private AudioInfo _toConfig;
        private AudioInfo _fromConfig;

        // From configs
        private int _fromChannels;
        private int _fromSampleLen;
        private long _fromChnSamples;

        // To configs
        private int _toChannels;
        private double _toChnFactor;
        private long _toChnSamples;

        private bool _firstRead;
        private bool _adaptiveEnabled;
        private long _totalLen;

        /// <summary>
        /// Audio sampler
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public AudioResampler(AudioInfo input, AudioInfo output)
        {
            _firstRead = true;
            _adaptiveEnabled = false;

            _toConfig = output;
            _fromConfig = input;

            _fromChannels = (int) _fromConfig.Channels;
            _fromSampleLen = Sample.GetSampleLen(_fromConfig.Format);

            _toChannels = (int) _toConfig.Channels;
            _toChnFactor = (double) _fromConfig.SampleRate / _toConfig.SampleRate;
        }

        /// <summary>
        /// Audio sampler
        /// </summary>
        /// <param name="output"></param>
        public AudioResampler(AudioInfo output)
        {
            _firstRead = true;
            _adaptiveEnabled = true;

            _toConfig = output;
            _fromConfig = output;

            _fromChannels = (int) _fromConfig.Channels;
            _fromSampleLen = Sample.GetSampleLen(_fromConfig.Format);

            _toChannels = (int) _toConfig.Channels;
            _toChnFactor = (double) _fromConfig.SampleRate / _toConfig.SampleRate;
        }

        /// <summary>
        /// Set adaptive input
        /// </summary>
        /// <param name="info"></param>
        internal override void SetAdaptiveInput(AudioInfo info)
        {
            // Adaptive not enabled
            if (!_adaptiveEnabled) return;

            _fromConfig = info;

            _fromChannels = (int) _fromConfig.Channels;
            _fromSampleLen = Sample.GetSampleLen(_fromConfig.Format);

            _toChannels = (int) _toConfig.Channels;
            _toChnFactor = (double) _fromConfig.SampleRate / _toConfig.SampleRate;
        }

        /// <summary>
        /// Get adaptive output
        /// </summary>
        /// <returns></returns>
        internal override AudioInfo GetAdaptiveOutput()
            => _toConfig;

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count)
        {
            // No need to sample
            if (_fromConfig.SampleRate == _toConfig.SampleRate
                && _fromConfig.Format == _toConfig.Format
                && _fromConfig.Channels == _toConfig.Channels)
            {
                _totalLen += count;
                base.Write(buffer, offset, count);
                return;
            }

            // Calculate sample length 
            _fromChnSamples = count / _fromSampleLen / _fromChannels;
            _toChnSamples = (long) ((double) _fromChnSamples / _fromConfig.SampleRate * _toConfig.SampleRate);

            // Resample the data
            for (long i = 0; i < _toChnSamples; ++i)
            {
                for (var j = 0; j < _toChannels; ++j)
                {
                    // Select the channel
                    var channel = j < _fromChannels
                        ? j
                        : j % _fromChannels;

                    // Calculate the sample scale
                    var sampleScale = _toChnFactor * i;
                    {
                        // Align to the channel
                        sampleScale -= sampleScale % _fromChannels;
                        sampleScale += channel;
                    }

                    // Seek to sample source
                    var samplePos = (int) (offset + sampleScale * _fromSampleLen * _fromChannels);
                    var sampleEnd = samplePos + _fromSampleLen;

                    // Read this sample
                    var sample = buffer[samplePos..sampleEnd];
                    var result = Sample.ConvertFormat(sample, _fromConfig.Format, _toConfig.Format);
                    {
                        _totalLen += result.Length;
                        base.Write(result, 0, result.Length);
                    }
                }
            }
        }

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_firstRead)
            {
                base.Position = 0;
                base.SetLength(_totalLen);
                _firstRead = false;
            }

            return base.Read(buffer, offset, count);
        }
    }
}
