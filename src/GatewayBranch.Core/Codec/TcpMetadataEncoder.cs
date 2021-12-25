using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace GatewayBranch.Core.Codec
{
    public class TcpMetadataEncoder : MessageToByteEncoder<byte[]>
    {
        protected override void Encode(IChannelHandlerContext context, byte[] message, IByteBuffer output)
        {
            output.WriteBytes(Unpooled.WrappedBuffer(message));
        }
    }
}
