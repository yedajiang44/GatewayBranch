dotnet publish ./src/GatewayBranch.Application/GatewayBranch.Application.csproj -c Release
docker build --pull --rm -f "Dockerfile" -t yedajiang44/gatewaybranch:latest "."