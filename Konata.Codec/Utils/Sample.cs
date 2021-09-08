using System;
using Konata.Codec.Audio;

namespace Konata.Codec.Utils
{
    /// <summary>
    /// Sample util
    /// </summary>
    public static class Sample
    {
        /// <summary>
        /// Get format align
        /// </summary>
        /// <returns></returns>
        public static int GetSampleLen(AudioFormat audioFormat)
            => audioFormat switch
            {
                AudioFormat.UnSigned8Bit => 1,
                AudioFormat.Signed16Bit => 2,
                AudioFormat.Signed32Bit => 4,
                AudioFormat.Float32Bit => 4,
                AudioFormat.Float64Bit => 8,
                _ => 0
            };

        /// <summary>
        /// Read sample data
        /// </summary>
        /// <param name="sample"></param>
        /// <param name="inputFormat"></param>
        /// <param name="outputFormat"></param>
        /// <returns></returns>
        public static byte[] ConvertFormat(byte[] sample,
            AudioFormat inputFormat, AudioFormat outputFormat)
        {
            // Convert sample to common format
            var common = inputFormat switch
            {
                AudioFormat.UnSigned8Bit =>
                    (double) (sbyte) sample[0] / sbyte.MaxValue,

                AudioFormat.Signed16Bit =>
                    (double) BitConverter.ToInt16(sample) / short.MaxValue,

                AudioFormat.Signed32Bit =>
                    (double) BitConverter.ToInt32(sample) / int.MaxValue,

                AudioFormat.Float32Bit =>
                    (double) BitConverter.ToSingle(sample) / float.MaxValue,

                AudioFormat.Float64Bit =>
                    BitConverter.ToDouble(sample),

                _ => 0
            };

            // Convert to output format
            return outputFormat switch
            {
                AudioFormat.UnSigned8Bit =>
                    BitConverter.GetBytes((byte) (common * sbyte.MaxValue)),

                AudioFormat.Signed16Bit =>
                    BitConverter.GetBytes((short) (common * short.MaxValue)),

                AudioFormat.Signed32Bit =>
                    BitConverter.GetBytes((int) (common * int.MaxValue)),

                AudioFormat.Float32Bit =>
                    BitConverter.GetBytes((float) common),

                AudioFormat.Float64Bit =>
                    BitConverter.GetBytes(common),

                _ => Array.Empty<byte>()
            };
        }
    }
}
