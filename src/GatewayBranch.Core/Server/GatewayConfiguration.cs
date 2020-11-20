using System;
using System.Collections.Generic;
using System.Text;

namespace GatewayBranch.Core.Server
{
    internal class GatewayConfiguration
    {
        public int TcpPort { get; set; } = 808;

        public int UdpPort { get; set; } = 808;

        public int WebSocketPort { get; set; } = 800;

        public int HttpPort { get; set; } = 801;

        public bool UseLibuv { get; set; }

        public int QuietPeriodSeconds { get; set; } = 1;

        public TimeSpan QuietPeriodTimeSpan => TimeSpan.FromSeconds(QuietPeriodSeconds);

        public int ShutdownTimeoutSeconds { get; set; } = 3;

        public TimeSpan ShutdownTimeoutTimeSpan => TimeSpan.FromSeconds(ShutdownTimeoutSeconds);

        public int SoBacklog { get; set; } = 8192;

        public int EventLoopCount { get; set; } = Environment.ProcessorCount;

        public int ReaderIdleTimeSeconds { get; set; } = 3600;

        public int WriterIdleTimeSeconds { get; set; } = 3600;

        public int AllIdleTimeSeconds { get; set; } = 3600;

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
