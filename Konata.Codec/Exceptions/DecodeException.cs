using System;

namespace Konata.Codec.Exceptions
{
    /// <summary>
    /// Decode exception
    /// </summary>
    public class DecodeException : Exception
    {
        /// <summary>
        /// Decode exception
        /// </summary>
        /// <param name="message"></param>
        public DecodeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Decode exception
        /// </summary>
        /// <param name="e"></param>
        public DecodeException(Exception e)
            : base("This stream cannot decode.", e)
        {
        }
    }
}
