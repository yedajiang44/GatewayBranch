using System;
using System.Linq;
using System.Threading;
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
    internal class TcpClientHandler : SimpleChannelInboundHandler<byte[]>
    {
        private readonly ILogger logger;
        private readonly GatewayConfiguration configuration;
        private readonly ITcpClientManager tcpClientManager;
        private readonly IServerSessionManager serverSessionManager;

        public TcpClientHandler(ILogger<TcpClientHandler> logger, ITcpClientManager tcpClientManager, IServerSessionManager serverSessionManager, IOptions<GatewayConfiguration> options)
        {
            this.logger = logger;
            this.tcpClientManager = tcpClientManager;
            this.serverSessionManager = serverSessionManager;
            configuration = options.Value;
        }

        public override void HandlerRemoved(IChannelHandlerContext context)
        {
            var channelId = context.Channel.Id.AsShortText();
            var endPoint = context.Channel.RemoteAddress;
            Parallel.ForEach(tcpClientManager.GetTcpClients().Where(x => x.GetSession(channelId) != default), async x =>
            {
                var count = 0L;
                while (true)
                {
                    count++;
                    try
                    {
                        if (count == 100)
                        {
                            if (logger.IsEnabled(LogLevel.Warning))
                            {
                                logger.LogWarning("已重试 {count} 次连接 {endPoint} 失败，即将取消重试", count, endPoint);
                            }
                            break;
                        }
                        var serverSessionId = x.GetSession(channelId)?.MatchId;
                        if (string.IsNullOrEmpty(serverSessionId))
                            break;
                        await x.ConnectAsync(endPoint, serverSessionId);
                        break;
                    }
                    catch (Exception e)
                    {
                        if (logger.IsEnabled(LogLevel.Trace))
                            logger.LogTrace(e, $"转发线路第 {count} 次断线重连 {endPoint} 服务器失败1秒后重试");
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                    }
                }
            });
            base.HandlerRemoved(context);
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, byte[] msg)
        {
            var channelId = ctx.Channel.Id.AsShortText();
            Parallel.ForEach(tcpClientManager.GetTcpClients().Where(x => configuration.BrabchServer.Where(item => item.NeedReply).Select(item => item.MatchId).Contains(x.Id)).Select(x => x.GetSession(channelId)?.MatchId).Where(x => !string.IsNullOrEmpty(x)), x =>
            {
                serverSessionManager.Send(x, msg).ContinueWith(task =>
                {
                    if (logger.IsEnabled(LogLevel.Error))
                        logger.LogError(task.Exception, "下发数据发生异常");
                    serverSessionManager.RemoveById(x);
                }, TaskContinuationOptions.NotOnRanToCompletion);
            });
            if (logger.IsEnabled(LogLevel.Trace))
                logger.LogTrace($"分发链路收到服务器 {ctx.Channel.RemoteAddress} 的数据 {msg.ToHexString()}");
        }

        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            var channelId = context.Channel.Id.AsShortText();
            switch (evt)
            {
                case IdleStateEvent readerIdle when readerIdle.State == IdleState.ReaderIdle:
                    tcpClientManager.GetTcpClients().Where(x => x.GetSession(channelId) != default).AsParallel().ForAll(x => x.CloseBySessionIdAsync(channelId));
                    context.CloseAsync();
                    break;
                case IdleStateEvent writerIdle when writerIdle.State == IdleState.WriterIdle:
                    tcpClientManager.GetTcpClients().Where(x => x.GetSession(channelId) != default).AsParallel().ForAll(x => x.CloseBySessionIdAsync(channelId));
                    context.CloseAsync();
                    break;
            }
            base.UserEventTriggered(context, evt);
        }
    }
}
