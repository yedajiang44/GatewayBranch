using DotNetty.Buffers;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Transport.Libuv;
using GatewayBranch.Core.Codec;
using GatewayBranch.Core.Handler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace GatewayBranch.Core.Server
{
    internal class TcpServerHost : IHostedService
    {
        readonly IServiceProvider serviceProvider;
        readonly ILogger logger;
        readonly GatewayConfiguration configuration;
        private IEventLoopGroup bossGroup;
        private IEventLoopGroup workerGroup;
        private IByteBufferAllocator serverBufferAllocator;

        public TcpServerHost(IServiceProvider serviceProvider, ILogger<TcpServerHost> logger, IOptions<GatewayConfiguration> configuration)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.configuration = configuration.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (configuration.TcpPort.Count == 0) return Task.CompletedTask;
            var bootstrap = new ServerBootstrap();
            serverBufferAllocator = new PooledByteBufferAllocator();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var dispatcher = new DispatcherEventLoopGroup();
                bossGroup = dispatcher;
                workerGroup = new WorkerEventLoopGroup(dispatcher);
                bootstrap.Channel<TcpServerChannel>();
                bootstrap
                    .Option(ChannelOption.SoReuseport, true)
                    .ChildOption(ChannelOption.SoReuseaddr, true);
            }
            else
            {
                bossGroup = new MultithreadEventLoopGroup(1);
                workerGroup = new MultithreadEventLoopGroup();
                bootstrap.Channel<TcpServerSocketChannel>();
            }
            bootstrap.Group(bossGroup, workerGroup);
            bootstrap
               .Option(ChannelOption.SoBacklog, configuration.SoBacklog)
               .ChildOption(ChannelOption.Allocator, serverBufferAllocator)
               .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
               {
                   var pipeline = channel.Pipeline;
                   using var scope = serviceProvider.CreateScope();
                   pipeline.AddLast(new IdleStateHandler(configuration.ReaderIdleTimeSeconds, configuration.WriterIdleTimeSeconds, configuration.AllIdleTimeSeconds));
                   pipeline.AddLast(scope.ServiceProvider.GetRequiredService<TcpMetadataDecoder>());
                   pipeline.AddLast(scope.ServiceProvider.GetRequiredService<TcpMetadataEncoder>());
                   pipeline.AddLast(scope.ServiceProvider.GetRequiredService<BranchTcpServerHandler>());
               }));
            Task.WaitAll(configuration.TcpPort.Select(x => bootstrap.BindAsync(x).ContinueWith(_ => logger.LogInformation($"TCP Server start at {IPAddress.Any}:{x}."))).ToArray());
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (configuration.TcpPort.Count == 0) return Task.CompletedTask;
            var quietPeriod = configuration.QuietPeriodTimeSpan;
            var shutdownTimeout = configuration.ShutdownTimeoutTimeSpan;
            return Task.WhenAll(bossGroup.ShutdownGracefullyAsync(quietPeriod, shutdownTimeout), workerGroup.ShutdownGracefullyAsync(quietPeriod, shutdownTimeout));
        }
    }
}
