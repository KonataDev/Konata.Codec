using System;
using System.IO;
using System.Threading.Tasks;
using MP3Sharp;

namespace Konata.Codec.Audio
{
    /// <summary>
    /// Mp3 codec
    /// </summary>
    public static class Mp3Codec
    {
        /// <summary>
        /// Decode mp3
        /// </summary>
        /// <param name="mp3Data"></param>
        /// <param name="pcmData"></param>
        /// <returns></returns>
        public static Task<bool> Decode(byte[] mp3Data, out byte[] pcmData)
        {
            pcmData = null;

            try
            {
                using var outStream = new MemoryStream(4096);
                using var memStream = new MemoryStream(mp3Data);
                using var mp3Stream = new MP3Stream(memStream);
                {
                    // Decode mp3 to pcm
                    mp3Stream.CopyToAsync(outStream).Wait();
                    pcmData = outStream.ToArray();

                    // Return data
                    return Task.FromResult(pcmData?.Length != 0);
                }
            }

            // Any exceptions
            catch
            {
                return Task.FromResult(false);
            }
        }
    }
}
