using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using GatewayBranch.Core.Codec;
using GatewayBranch.Core.Handler;
using GatewayBranch.Core.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace GatewayBranch.Core.Client
{
    internal class TcpClient : ITcpClient
    {
        public string Id { get; set; } = "default";
        private readonly GatewayConfiguration configuration;
        private readonly MultithreadEventLoopGroup eventLoopGroup;
        private readonly Bootstrap bootstrap;
        private readonly ITcpClientSessionManager sessionManager;
        private readonly ILogger logger;

        public TcpClient(IServiceProvider serviceProvider, ITcpClientSessionManager sessionManager, ILogger<TcpClient> logger, IOptions<GatewayConfiguration> configuration)
        {
            this.logger = logger;
            this.sessionManager = sessionManager;
            this.configuration = configuration.Value;
            eventLoopGroup = new MultithreadEventLoopGroup();
            bootstrap = new Bootstrap().Group(eventLoopGroup)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Option(ChannelOption.ConnectTimeout, TimeSpan.FromSeconds(30))
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    var scope = serviceProvider.CreateScope().ServiceProvider;
                    IChannelPipeline pipeline = channel.Pipeline;
                    pipeline.AddLast(new IdleStateHandler(this.configuration.BrabchServerReaderIdleTimeSeconds, this.configuration.BrabchServerWriterIdleTimeSeconds, this.configuration.BrabchServerAllIdleTimeSeconds));
                    pipeline.AddLast(scope.GetRequiredService<TcpMetadataDecoder>());
                    pipeline.AddLast(scope.GetRequiredService<TcpMetadataEncoder>());
                    pipeline.AddLast(scope.GetRequiredService<TcpClientHandler>());
                }));
        }
        public async Task<ISession> ConnectAsync(EndPoint endPoint, string matchId)
        {
            var channel = await bootstrap.ConnectAsync(endPoint);
            ISession session = new Session { Channel = channel, MatchId = matchId };
            sessionManager.Add(session);
            return session;
        }
        public Task CloseAsync(string matchId)
        {
            return Task.Run(() => sessionManager.RemoveByMatchId(matchId));
        }
        public Task CloseBySessionIdAsync(string sessionId)
        {
            logger.LogInformation($"断开分发链路 {sessionId}");
            return Task.Run(() => sessionManager.RemoveById(sessionId));
        }

        public Task Send(string matchId, byte[] data) => sessionManager.GetSession(matchId).Send(data);

        public ISession GetSession(string sessionId) => sessionManager.GetSessionById(sessionId);
        public ISession GetSessionByServerSessionId(string sessionId) => sessionManager.GetSession(sessionId);

        public IEnumerable<ISession> Sesions() => sessionManager.GetSessions();
    }
    public interface ITcpClient
    {
        public string Id { get; set; }
        Task<ISession> ConnectAsync(EndPoint endPoint, string matchId = null);
        Task CloseAsync(string matchId);
        Task CloseBySessionIdAsync(string sessionId);
        ISession GetSession(string sessionId);
        ISession GetSessionByServerSessionId(string sessionId);
        IEnumerable<ISession> Sesions();
        Task Send(string matchId, byte[] data);
    }
}
