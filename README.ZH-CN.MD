# GatewayBranch

GatewayBranch 是一个基于DotNetty的轻量级网关转发工具

![workflow](./imgs/workflow.png)

# 如何使用

导入命名空间

```c#
using GatewayBranch.Core;
```

添加中间件
```c#
services.UseGatewayBranch(hostContext.Configuration);
```

配置 appsettings.json
``` json
  "Gateway": {
    "TcpPort": 2012, // listener port
    "BrabchServer": [
      {
        "Ip": "127.0.0.1", // branch server ip
        "Port": 808, // branch server port
        "NeedReply": true // if `true` branch port downstream data,else do nothing
      },
      {
        "Ip": "127.0.0.1",
        "Port": 809,
        "NeedReply": true
      }
    ]
  }
```

# Roadmap

- 添加 udp 支持
- 添加连接管理器接口