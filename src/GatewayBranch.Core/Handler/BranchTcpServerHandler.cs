using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using GatewayBranch.Core.Client;
using GatewayBranch.Core.Server;
using GatewayBranch.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace GatewayBranch.Core.Handler
{
    internal class BranchTcpServerHandler : SimpleChannelInboundHandler<byte[]>
    {
        readonly ILogger logger;
        readonly ITcpClientManager tcpClientManager;
        readonly GatewayConfiguration configuration;
        readonly IServerSessionManager serverSessionManager;
        public BranchTcpServerHandler(ILogger<BranchTcpServerHandler> logger, IServerSessionManager serverSessionManager, ITcpClientFactory tcpClientFactory, ITcpClientManager tcpClientManager, IOptions<GatewayConfiguration> options)
        {
            this.logger = logger;
            this.tcpClientManager = tcpClientManager;
            this.serverSessionManager = serverSessionManager;
            configuration = options.Value;
            Parallel.ForEach(configuration.BrabchServer, x =>
            {
                tcpClientFactory.CreateTcpClient(x.IpAdress);
            });
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            base.ChannelActive(context);
            serverSessionManager.Add(new Server.Session { Channel = context.Channel });
            var channelId = context.Channel.Id.AsShortText();
            Parallel.ForEach(configuration.BrabchServer, x =>
            {
                var tcpClient = tcpClientManager.GetTcpClient(x.IpAdress);
                tcpClient.ConnectAsync(x.Ip, x.Port, channelId);
            });
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            serverSessionManager.RemoveById(context.Channel.Id.AsShortText());
            Parallel.ForEach(configuration.BrabchServer, x =>
            {
                var client = tcpClientManager.GetTcpClient(x.IpAdress);
                client.CloseAsync(context.Channel.Id.AsShortText());
            });
            base.ChannelInactive(context);
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, byte[] msg)
        {
            var channelId = ctx.Channel.Id.AsShortText();
            Parallel.ForEach(configuration.BrabchServer, x =>
            {
                var client = tcpClientManager.GetTcpClient(x.IpAdress);
                var session = client.GetSessionByServerSessionId(channelId);
                if (session != default)
                    session.Send(msg);
                else
                    client.ConnectAsync(x.Ip, x.Port, channelId);
            });

            if (logger.IsEnabled(LogLevel.Trace))
                logger.LogTrace($"监听链路 {ctx.Channel.RemoteAddress} 收到：{msg.ToHexString()}");
        }
        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            switch (evt)
            {
                case IdleStateEvent readerIdle when readerIdle.State == IdleState.ReaderIdle:
                    serverSessionManager.RemoveById(context.Channel.Id.AsShortText());
                    context.CloseAsync();
                    break;
                case IdleStateEvent writerIdle when writerIdle.State == IdleState.WriterIdle:
                    serverSessionManager.RemoveById(context.Channel.Id.AsShortText());
                    context.CloseAsync();
                    break;
            }
            base.UserEventTriggered(context, evt);
        }
    }
}
