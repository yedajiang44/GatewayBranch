using GatewayBranch.Core.Client;
using GatewayBranch.Core.Codec;
using GatewayBranch.Core.Handler;
using GatewayBranch.Core.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GatewayBranch.Core
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddGatewayBranch(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddScoped<TcpMetadataDecoder>()
                .AddScoped<TcpMetadataEncoder>()
                .AddScoped<BranchTcpServerHandler>()
                .AddScoped<TcpClientHandler>()
                .AddSingleton<ITcpClientManager, TcpClientManager>()
                .AddScoped<ITcpClientSessionManager, TcpClientSessionManager>()
                .AddScoped<ITcpClient, TcpClient>()
                .AddSingleton<ITcpClientFactory, TcpClientFactory>()
                .AddSingleton<IServerSessionManager, ServerSessionManager>()
                .Configure<GatewayConfiguration>(configuration.GetSection("Gateway"))
                .AddHostedService<TcpServerHost>();
        }
    }
}
