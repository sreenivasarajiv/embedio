﻿namespace Unosquare.Labs.EmbedIO.Constants
{
    /// <summary>
    /// Specifies the compression method used to compress a message on
    /// the WebSocket connection.
    /// </summary>
    /// <remarks>
    /// The compression methods that can be used are defined in
    /// <see href="https://tools.ietf.org/html/rfc7692">
    /// Compression Extensions for WebSocket</see>.
    /// </remarks>
    public enum CompressionMethod : byte
    {
        /// <summary>
        /// Specifies non compression.
        /// </summary>
        None,

        /// <summary>
        /// Specifies DEFLATE.
        /// </summary>
        Deflate,

        /// <summary>
        /// Specifies GZIP.
        /// </summary>
        Gzip
    }
}
