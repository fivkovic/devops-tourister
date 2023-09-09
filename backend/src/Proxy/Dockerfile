FROM mcr.microsoft.com/dotnet/sdk:8.0-preview AS builder

# Setup working directory for the project
WORKDIR /src
COPY ./src/Shared/Shared.csproj ./Shared/
COPY ./src/Proxy/src/Proxy.csproj ./Proxy/src/

# Restore nuget packages
RUN dotnet restore ./Proxy/src/Proxy.csproj

# Copy project files
COPY ./src/Shared ./Shared/
COPY ./src/Proxy/src  ./Proxy/src/

# Build project with Release configuration and no restore (already done restore)

RUN ls
RUN dotnet build  -c Release --no-restore ./Proxy/src/Proxy.csproj

WORKDIR /src/Proxy/src

# Publish project to output folder and no build (already done build)
RUN dotnet publish -c Release --no-build -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0-preview

# Setup working directory for the project
WORKDIR /app
COPY --from=builder /src/Proxy/src/out  .

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_HTTP_PORT=80
ENV ASPNETCORE_ENVIRONMENT=docker

EXPOSE 80

ENTRYPOINT ["dotnet", "Proxy.dll"]