using GatewayBranch.Core;
using Microsoft.OpenApi.Models;
using NLog.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.Services
.AddEndpointsApiExplorer()
.AddAuthorization()
.AddGatewayBranch(builder.Configuration)
.AddLogging(logger => logger.ClearProviders().AddNLog(new NLogLoggingConfiguration(builder.Configuration.GetSection("NLog"))))
.AddControllers();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen(o =>
    {
        o.EnableAnnotations();
        o.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "公开接口",
            Description = "第一版公开接口",
            Version = "v1",
            Contact = new OpenApiContact
            {
                Name = "yedajiang44",
                Email = "602830483@qq.com",
                Url = new Uri("http://www.github.com/yedajiang44"),
            }
        });
    });
}

var app = builder.Build();
app.UseRouting()
.UseAuthentication()
.UseAuthorization()
.UseEndpoints(endpoints => endpoints.MapControllers());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger()
    .UseSwaggerUI(c =>
    {
        c.RoutePrefix = string.Empty;
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "公开接口");
    });
}

await app.RunAsync();