using System.IO;

namespace Konata.Codec.Audio
{
    /// <summary>
    /// Audio stream
    /// </summary>
    public abstract class AudioStream : MemoryStream
    {
        /// <summary>
        /// Set adaptive input
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        internal virtual void SetAdaptiveInput(AudioInfo info)
        {
        }

        /// <summary>
        /// Get adaptive output
        /// </summary>
        /// <returns></returns>
        internal abstract AudioInfo GetAdaptiveOutput();
    }
}
