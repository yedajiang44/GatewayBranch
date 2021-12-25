using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace GatewayBranch.Core.Server
{
    /// <summary>
    /// Gateway configuration.
    /// </summary>
    internal class GatewayConfiguration
    {
        /// <summary>
        /// Gets or sets the tcp ports.
        /// </summary>
        /// <value>
        /// The tcp ports.
        /// </value>
        public List<int> TcpPort { get; set; } = new List<int>();
        /// <summary>
        /// Gets or sets the udp ports.
        /// </summary>
        /// <value>
        /// The udp ports.
        /// </value>
        public List<int> UdpPort { get; set; } = new List<int>();
        /// <summary>
        /// Gets or sets the use libuv.
        /// </summary>
        /// <value>
        /// The use libuv.
        /// </value>
        public bool UseLibuv { get; set; }
        /// <summary>
        /// Gets or sets the quiet period seconds.
        /// </summary>
        /// <value>
        /// The quiet period seconds.
        /// </value>
        public int QuietPeriodSeconds { get; set; } = 1;

        public TimeSpan QuietPeriodTimeSpan => TimeSpan.FromSeconds(QuietPeriodSeconds);
        /// <summary>
        /// Gets or sets the shutdown timeout seconds.
        /// </summary>
        /// <value>
        /// The shutdown timeout seconds.
        /// </value>
        public int ShutdownTimeoutSeconds { get; set; } = 3;

        public TimeSpan ShutdownTimeoutTimeSpan => TimeSpan.FromSeconds(ShutdownTimeoutSeconds);
        /// <summary>
        /// Gets or sets the so backlog.
        /// </summary>
        /// <value>
        /// The so backlog.
        /// </value>
        public int SoBacklog { get; set; } = 8192;
        /// <summary>
        /// Gets or sets the event loop count.
        /// </summary>
        /// <value>
        /// The event loop count.
        /// </value>
        public int EventLoopCount { get; set; } = Environment.ProcessorCount;
        /// <summary>
        /// Gets or sets the reader idle time seconds.
        /// </summary>
        /// <value>
        /// The reader idle time seconds.
        /// </value>
        public int ReaderIdleTimeSeconds { get; set; } = 300;
        /// <summary>
        /// Gets or sets the writer idle time seconds.
        /// </summary>
        /// <value>
        /// The writer idle time seconds.
        /// </value>
        public int WriterIdleTimeSeconds { get; set; } = 300;
        /// <summary>
        /// Gets or sets the all idle time seconds.
        /// </summary>
        /// <value>
        /// The all idle time seconds.
        /// </value>
        public int AllIdleTimeSeconds { get; set; }
        /// <summary>
        /// Gets or sets the brabch server reader idle time seconds.
        /// </summary>
        /// <value>
        /// The brabch server reader idle time seconds.
        /// </value>
        public int BrabchServerReaderIdleTimeSeconds { get; set; } = 300;
        /// <summary>
        /// Gets or sets the brabch server writer idle time seconds.
        /// </summary>
        /// <value>
        /// The brabch server writer idle time seconds.
        /// </value>
        public int BrabchServerWriterIdleTimeSeconds { get; set; } = 300;
        /// <summary>
        /// Gets or sets the brabch server all idle time seconds.
        /// </summary>
        /// <value>
        /// The brabch server all idle time seconds.
        /// </value>
        public int BrabchServerAllIdleTimeSeconds { get; set; }
        /// <summary>
        /// Gets or sets the brabch server.
        /// </summary>
        /// <value>
        /// The brabch server.
        /// </value>
        public List<Server> BrabchServer { get; set; }
        internal class Server
        {
            public string Ip { get; set; }
            public int Port { get; set; }
            public bool NeedReply { get; set; }
            public string MatchId => $"{Ip}:{Port}";
            private readonly Lazy<EndPoint> IpAdress;
            public EndPoint Host => IpAdress.Value;
            public Server()
            {
                IpAdress = new Lazy<EndPoint>(() => new IPEndPoint(Array.Find(Dns.GetHostEntry(Ip).AddressList, x => x.AddressFamily == AddressFamily.InterNetwork), Port));
            }
        }
    }
}
