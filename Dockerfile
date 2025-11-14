FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

RUN dotnet publish src/GatewayBranch.Application/GatewayBranch.Application.csproj \
    -c Release -o /app/publish \
    --self-contained false \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV TZ=Asia/Shanghai

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "GatewayBranch.Application.dll"]
