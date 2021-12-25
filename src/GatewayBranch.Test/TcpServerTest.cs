using System;
using GatewayBranch.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Xunit;

namespace GatewayBranch.Test
{
    [Collection("Tcp Server Test")]
    public class TcpServerTest
    {
        private readonly IHost host;
        public TcpServerTest()
        {
            host = Host.CreateDefaultBuilder(Array.Empty<string>())
              .ConfigureServices((hostContext, services) =>
              {
                  services
                  .AddGatewayBranch(hostContext.Configuration)
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
        ~TcpServerTest()
        {
            host.StopAsync();
        }
    }
}
