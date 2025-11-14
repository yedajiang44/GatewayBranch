# GatewayBranch

GatewayBranch 是一个基于 DotNetty 的轻量级网关转发工具

![workflow](./imgs/workflow.png)

# 如何使用

导入命名空间

```c#
using GatewayBranch.Core;
```

添加中间件

```c#
services.AddGatewayBranch(hostContext.Configuration);
```

配置 appsettings.json

```json
  "Gateway": {
    "TcpPort": [2012,2013], // 监听端口集合
    "BrabchServer": [
      {
        "Ip": "127.0.0.1", // 分支服务器Ip或域名
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

# 在 docker 中运行

为方便使用，已创建 docker 镜像 [yedajiang44/gatewaybranch](https://github.com/yedajiang44/GatewayBranch/pkgs/container/gatewaybranch)

- 下载 `docker-compose.yml` 文件
- 执行 `docker-compose up -d`

## 注意

> 如果要映射配置文件，则需要创建相应的文件

- `appsettings.json` content see [appsettings.json](./src/GatewayBranch.Application/appsettings.json)，具体配置项请参见[GatewayConfiguration](./src/GatewayBranch.Core/Server/GatewayConfiguration.cs)