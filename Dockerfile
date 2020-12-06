FROM mcr.microsoft.com/dotnet/runtime:3.1
WORKDIR /app
COPY ./src/GatewayBranch.Application/bin/Release/netcoreapp3.1 . 
#设置端口
EXPOSE 2012
ENTRYPOINT ["dotnet", "GatewayBranch.Application.dll"] 