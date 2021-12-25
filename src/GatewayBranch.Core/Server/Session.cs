using DotNetty.Transport.Channels;
using System;
using System.Threading.Tasks;

namespace GatewayBranch.Core.Server
{
    internal class Session : ISession
    {
        public IChannel Channel { get; set; }
        public string Id => Channel?.Id.AsShortText();

        public void Dispose()
        {
            Channel.DisconnectAsync();
            Channel = null;
        }

        public Task Send(byte[] data) => Channel.WriteAndFlushAsync(data);
    }
    public interface ISession : IDisposable
    {
        public string Id { get; }
        Task Send(byte[] data);
        public enum SessionStatus
        {
            Offline
        }
    }
}
