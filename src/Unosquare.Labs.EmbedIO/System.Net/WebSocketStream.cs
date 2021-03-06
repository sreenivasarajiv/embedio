﻿namespace Unosquare.Net
{
    using Labs.EmbedIO;
    using Labs.EmbedIO.Constants;
    using System.Collections.Generic;
    using System.IO;

    internal class WebSocketStream : MemoryStream
    {
        internal static readonly byte[] EmptyBytes = new byte[0];
        internal static readonly int FragmentLength = 1016;

        private readonly CompressionMethod _compression;
        private readonly Opcode _opcode;
        private readonly bool _isClient;

        public WebSocketStream(byte[] data, Opcode opcode, CompressionMethod compression, bool isClient)
            : base(data)
        {
            _compression = compression;
            _opcode = opcode;
            _isClient = isClient;
        }

        public IEnumerable<WebSocketFrame> GetFrames()
        {
            var compressed = _compression != CompressionMethod.None;
            Stream stream = _compression != CompressionMethod.None
                ? this.Compress(_compression)
                : this;

            var len = stream.Length;

            /* Not fragmented */

            if (len == 0)
            {
                yield return new WebSocketFrame(Fin.Final, _opcode, EmptyBytes, compressed, _isClient);
                yield break;
            }

            var quo = len / FragmentLength;
            var rem = (int)(len % FragmentLength);

            byte[] buff;

            if (quo == 0)
            {
                buff = new byte[rem];

                if (stream.Read(buff, 0, rem) == rem)
                    yield return new WebSocketFrame(Fin.Final, _opcode, buff, compressed, _isClient);

                yield break;
            }

            buff = new byte[FragmentLength];
            if (quo == 1 && rem == 0)
            {
                if (stream.Read(buff, 0, FragmentLength) == FragmentLength)
                    yield return new WebSocketFrame(Fin.Final, _opcode, buff, compressed, _isClient);

                yield break;
            }

            /* Send fragmented */

            // Begin
            if (stream.Read(buff, 0, FragmentLength) != FragmentLength)
                yield break;

            yield return new WebSocketFrame(Fin.More, _opcode, buff, compressed, _isClient);

            var n = rem == 0 ? quo - 2 : quo - 1;
            for (var i = 0; i < n; i++)
            {
                if (stream.Read(buff, 0, FragmentLength) != FragmentLength)
                    yield break;

                yield return new WebSocketFrame(Fin.More, Opcode.Cont, buff, compressed, _isClient);
            }

            // End
            if (rem == 0)
                rem = FragmentLength;
            else
                buff = new byte[rem];

            if (stream.Read(buff, 0, rem) == rem)
                yield return new WebSocketFrame(Fin.Final, Opcode.Cont, buff, compressed, _isClient);
        }
    }
}