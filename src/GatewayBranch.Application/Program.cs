using GatewayBranch.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.Services
.AddEndpointsApiExplorer()
.AddSwaggerGen(o => o.EnableAnnotations())
.AddAuthorization()
.AddGatewayBranch(builder.Configuration)
.AddLogging(logger => logger.ClearProviders().AddNLog(new NLogLoggingConfiguration(builder.Configuration.GetSection("NLog"))))
.AddControllers();

var app = builder.Build();
app.UseRouting()
.UseSwagger()
.UseSwaggerUI()
.UseAuthentication()
.UseAuthorization()
.UseEndpoints(endpoints => endpoints.MapControllers());
app.Run();