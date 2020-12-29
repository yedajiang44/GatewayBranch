dotnet publish ./src/GatewayBranch.Application/GatewayBranch.Application.csproj -c Release
docker build --pull --rm --no-cache -f "Dockerfile" -t yedajiang44/gatewaybranch "."