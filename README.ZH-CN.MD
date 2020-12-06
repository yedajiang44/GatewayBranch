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
    "TcpPort": 2012, // 监听端口
    "BrabchServer": [
      {
        "Ip": "127.0.0.1", // 分支服务器Ip
        "Port": 808, // 分支服务器 port
        "NeedReply": true // 如果为 `true` 该分支的下行数据将通过 `TcpPort` 下发至终端,否则将过滤掉
      },
      {
        "Ip": "127.0.0.1",
        "Port": 809,
        "NeedReply": true
      }
    ]
  }
```

# 在docker中运行
- 下载`docker-compose.yml`文件
- 执行`docker-compose up -d`

# Roadmap

- 添加 udp 支持
- 添加连接管理器接口