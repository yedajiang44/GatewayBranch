using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Collections.Generic;

namespace GatewayBranch.Core.Codec
{
    public class TcpMetadataDecoder : ByteToMessageDecoder
    {
        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            byte[] buffer = new byte[input.ReadableBytes];
            input.ReadBytes(buffer, 0, input.ReadableBytes);
            output.Add(buffer);
        }
    }
}
