FROM mcr.microsoft.com/dotnet/runtime:7.0
WORKDIR /app
COPY .output . 
#设置端口
EXPOSE 80
ENTRYPOINT ["dotnet", "GatewayBranch.Application.dll"] 