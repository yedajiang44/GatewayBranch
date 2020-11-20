using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GatewayBranch.Core.Client
{
    internal class TcpClientFactory : ITcpClientFactory
    {
        readonly IServiceProvider serviceProvider;
        readonly ITcpClientManager tcpClientManager;

        public TcpClientFactory(IServiceProvider serviceProvider, ITcpClientManager tcpClientManager)
        {
            this.serviceProvider = serviceProvider;
            this.tcpClientManager = tcpClientManager;
        }

        public ITcpClient CreateTcpClient(string clientId, bool fromCache = true, bool addManager = true)
        {
            if (string.IsNullOrEmpty(clientId)) throw new NullReferenceException($"the {nameof(clientId)} is null or empty");
            var client = fromCache ? tcpClientManager.GetTcpClient(clientId) : default;
            if (client == default)
            {
                client = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ITcpClient>();
                client.Id = clientId;
                if (addManager)
                    tcpClientManager.Add(client);
            }
            return client;
        }
    }
    public interface ITcpClientFactory
    {
        ITcpClient CreateTcpClient(string clientId = "default", bool fromCache = true, bool addManager = true);
    }
}
