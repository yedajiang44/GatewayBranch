﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace GatewayBranch.Core.Client
{
    internal class TcpClientManager : ITcpClientManager
    {
        private readonly ConcurrentDictionary<string, ITcpClient> clients = new ConcurrentDictionary<string, ITcpClient>();
        public void Add(ITcpClient client)
        {
            if (string.IsNullOrEmpty(client.Id)) throw new NullReferenceException($"the {nameof(client.Id)} is null or empty");
            clients.AddOrUpdate(client.Id, client, (_, __) => client);
        }

        public ITcpClient GetTcpClient(string id)
        {
            clients.TryGetValue(id, out ITcpClient client);
            return client;
        }

        public IEnumerable<ITcpClient> GetTcpClients() => clients.Values.ToList();
    }

    public interface ITcpClientManager
    {
        void Add(ITcpClient client);
        ITcpClient GetTcpClient(string id = "default");
        IEnumerable<ITcpClient> GetTcpClients();
    }
}
