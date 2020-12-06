# GatewayBranch

GatewayBranch is a lightweight gateway forwarding tool developed based on DotNet

![workflow](./imgs/workflow.png)

# How to use

import package

```c#
using GatewayBranch.Core;
```

add service
```c#
services.UseGatewayBranch(hostContext.Configuration);
```

config appsettings.json
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
# run in docker
- download `docker-compose.yml`
- exec `docker-compose up -d`

# Roadmap

- add udp
- add session mananger webapi