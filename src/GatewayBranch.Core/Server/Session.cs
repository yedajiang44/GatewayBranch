﻿using System;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;

namespace GatewayBranch.Core.Server
{
    internal class Session : ISession
    {
        public IChannel Channel { get; set; }
        public string Id => Channel?.Id.AsShortText();

        public Task CloseAsync()
        {
            return Channel.CloseAsync();
        }

        public void Dispose()
        {
            Channel = null;
        }

        public Task Send(byte[] data) => Channel.WriteAndFlushAsync(data);
    }
    public interface ISession : IDisposable
    {
        public string Id { get; }
        Task Send(byte[] data);
        Task CloseAsync();
        public enum SessionStatus
        {
            Offline
        }
    }
}
