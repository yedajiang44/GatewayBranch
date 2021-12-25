using DotNetty.Transport.Channels;
using System;
using System.Threading.Tasks;

namespace GatewayBranch.Core.Client
{
    internal class Session : ISession
    {
        public IChannel Channel { get; set; }
        public string Id => Channel?.Id.AsShortText();

        public string MatchId { get; set; }

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
        public string MatchId { get; set; }
        Task Send(byte[] data);
    }
}
