using System;
using System.IO;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Konata.Codec.Audio
{
    /// <summary>
    /// Audio pipeline
    /// </summary>
    public class AudioPipeline
        : IDisposable, IEnumerable
    {
        private readonly List<Stream> _streams;

        /// <summary>
        /// AudioPipeline
        /// </summary>
        public AudioPipeline()
        {
            _streams = new();
        }

        /// <summary>
        /// Start pipeline
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Start()
        {
            return await Task.Run(() =>
            {
                // Process each stream
                for (var i = 0; i < _streams.Count - 1; ++i)
                {
                    // Adaptive audio format
                    if (_streams[i] is AudioStream x
                        && _streams[i + 1] is AudioStream y)
                    {
                        y.SetAdaptiveInput(x.GetAdaptiveOutput());
                    }

                    // Pass data to next stream
                    _streams[i].CopyTo(_streams[i + 1]);
                }

                return true;
            });
        }

        /// <summary>
        /// Add stream
        /// </summary>
        /// <param name="stream"></param>
        public void Add(Stream stream)
            => _streams.Add(stream);

        /// <inheritdoc />
        public IEnumerator GetEnumerator()
            => _streams.GetEnumerator();

        /// <inheritdoc />
        public void Dispose()
        {
            // Dispose all streams
            foreach (var i in _streams) i.Dispose();
        }
    }
}
