using System;

namespace Konata.Codec.Exceptions;

/// <summary>
/// Encode exception
/// </summary>
public class EncodeException : Exception
{
    /// <summary>
    /// Encode exception
    /// </summary>
    /// <param name="message"></param>
    public EncodeException(string message)
        : base(message)
    {
    }
}
