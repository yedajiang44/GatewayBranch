using System;
using System.Collections.Generic;
using System.Text;

namespace GatewayBranch.Core.Server
{
    /// <summary>
    /// Gateway configuration.
    /// </summary>
    internal class GatewayConfiguration
    {
        /// <summary>
        /// Gets or sets the tcp port.
        /// </summary>
        /// <value>
        /// The tcp port.
        /// </value>
        public int TcpPort { get; set; } = 808;
        /// <summary>
        /// Gets or sets the udp port.
        /// </summary>
        /// <value>
        /// The udp port.
        /// </value>
        public int UdpPort { get; set; } = 808;
        /// <summary>
        /// Gets or sets the web socket port.
        /// </summary>
        /// <value>
        /// The web socket port.
        /// </value>
        public int WebSocketPort { get; set; } = 800;
        /// <summary>
        /// Gets or sets the http port.
        /// </summary>
        /// <value>
        /// The http port.
        /// </value>
        public int HttpPort { get; set; } = 801;
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
            public string IpAdress => $"{Ip}:{Port}";
        }
    }
}
