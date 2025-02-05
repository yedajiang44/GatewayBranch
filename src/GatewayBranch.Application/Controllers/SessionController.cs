using GatewayBranch.Application.Dtos.Enums;
using GatewayBranch.Core.Client;
using GatewayBranch.Core.Server;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GatewayBranch.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionController(ITcpClientManager tcpClientManager, IServerSessionManager serverSessionManager) : Controller
{
    [HttpGet("{type}")]
    [SwaggerOperation("获取连接", "根据不同的操作类型获取对应的连接信息")]
    public IActionResult Sessions([SwaggerParameter("0: the sessions of server<br/>1: the sessions of branch")] OperateType type)
    {
        return Json(type switch
        {
            OperateType.Server => serverSessionManager.GetSessions(),
            OperateType.Branch => tcpClientManager.GetTcpClients().SelectMany(x => x.Sesions()),
            _ => throw new InvalidProgramException()
        });
    }

    [HttpGet("{type}/[action]/{id}")]
    [SwaggerOperation("关闭连接", "关闭指定连接")]
    public Task<bool> Close([SwaggerParameter("0: the sessions of server<br/>1: the sessions of branch")] OperateType type, [SwaggerParameter("sesion id")] string id)
    {
        return type switch
        {
            OperateType.Server => new Func<Task<bool>>(async () =>
            {
                await serverSessionManager.GetSessionById(id)?.CloseAsync();
                return true;
            })(),
            OperateType.Branch => new Func<Task<bool>>(async () =>
            {
                await Parallel.ForEachAsync(tcpClientManager.GetTcpClients().Select(x => x.GetSession(id)), async (x, _) => await x.CloseAsync());
                return true;
            })(),
            _ => throw new ArgumentException("invalid parameter", nameof(type))
        };
    }
}