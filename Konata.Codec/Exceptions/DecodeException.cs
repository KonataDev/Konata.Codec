using System;

namespace Konata.Codec.Exceptions;

/// <summary>
/// Decode exception
/// </summary>
public class DecodeException : Exception
{
    /// <summary>
    /// Decode exception
    /// </summary>
    /// <param name="message"></param>
    public DecodeException(string message, Exception inner = null)
        : base(message, inner)
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
