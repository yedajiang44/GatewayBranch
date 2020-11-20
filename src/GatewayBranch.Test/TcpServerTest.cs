using GatewayBranch.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Configuration;
using System.Threading;
using Xunit;

namespace GatewayBranch.Test
{
    [Collection("Tcp Server Test")]
    public class TcpServerTest : IDisposable
    {
        readonly IHost host;
        public TcpServerTest()
        {
            host = Host.CreateDefaultBuilder(new string[] { })
              .ConfigureServices((hostContext, services) =>
              {
                  services
                  .UseGatewayBranch(hostContext.Configuration)
                  .AddLogging(logger =>
                  {
                      logger.ClearProviders();
                      logger.AddNLog(hostContext.Configuration);
                  });
              }).Build();
            host.StartAsync();
        }
        [Fact]
        public void Test1()
        {
            //Thread.Sleep(1000 * 60 * 10);
        }
        public void Dispose()
        {
            host.StopAsync();
        }
    }
}
