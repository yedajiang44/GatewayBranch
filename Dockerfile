FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY .output .
ENTRYPOINT ["./GatewayBranch.Application"]