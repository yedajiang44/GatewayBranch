using System.Threading.Tasks;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using GatewayBranch.Core.Client;
using GatewayBranch.Core.Server;
using GatewayBranch.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GatewayBranch.Core.Handler
{
    internal class BranchTcpServerHandler : SimpleChannelInboundHandler<byte[]>
    {
        private readonly ILogger logger;
        private readonly ITcpClientManager tcpClientManager;
        private readonly GatewayConfiguration configuration;
        private readonly IServerSessionManager serverSessionManager;
        public BranchTcpServerHandler(ILogger<BranchTcpServerHandler> logger, IServerSessionManager serverSessionManager, ITcpClientFactory tcpClientFactory, ITcpClientManager tcpClientManager, IOptions<GatewayConfiguration> options)
        {
            this.logger = logger;
            this.tcpClientManager = tcpClientManager;
            this.serverSessionManager = serverSessionManager;
            configuration = options.Value;
            Parallel.ForEach(configuration.BrabchServer, x => tcpClientFactory.CreateTcpClient(x.MatchId));
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            base.ChannelActive(context);
            serverSessionManager.Add(new Server.Session { Channel = context.Channel });
            var channelId = context.Channel.Id.AsShortText();
            Parallel.ForEach(configuration.BrabchServer, x =>
            {
                var tcpClient = tcpClientManager.GetTcpClient(x.MatchId);
                tcpClient.ConnectAsync(x.Host, channelId);
            });
            context.Channel.CloseCompletion.ContinueWith((_, state) =>
            {
                serverSessionManager.RemoveById(state as string);
                Parallel.ForEach(configuration.BrabchServer, x =>
                {
                    var client = tcpClientManager.GetTcpClient(x.MatchId);
                    client.CloseAsync(context.Channel.Id.AsShortText());
                });
            }, context.Channel.Id.AsShortText());
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, byte[] msg)
        {
            var channelId = ctx.Channel.Id.AsShortText();
            Parallel.ForEach(configuration.BrabchServer, x =>
            {
                var client = tcpClientManager.GetTcpClient(x.MatchId);
                var session = client.GetSessionByServerSessionId(channelId);
                if (session != default)
                    session.Send(msg);
                else
                    client.ConnectAsync(x.Host, channelId);
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
