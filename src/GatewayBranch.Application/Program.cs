using GatewayBranch.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System.Threading.Tasks;

namespace GatewayBranch.Application
{
    class Program
    {
        static Task Main(string[] args) => Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services
                .AddGatewayBranch(hostContext.Configuration)
                .AddLogging(logger =>
                {
                    logger.ClearProviders().AddNLog(new NLogLoggingConfiguration(hostContext.Configuration.GetSection("NLog")));
                });
            }).RunConsoleAsync();
    }
}
