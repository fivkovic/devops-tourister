FROM mcr.microsoft.com/dotnet/sdk:8.0-preview AS builder

# Setup working directory for the project
WORKDIR /
COPY ./src/Shared/Shared.csproj ./Shared/
COPY ./src/Services/Identity/src/Identity/Identity.csproj ./Services/Identity/src/Identity/
COPY ./src/Services/Identity/src/Identity.API/Identity.API.csproj ./Services/Identity/src/Identity.API/

# Restore nuget packages
RUN dotnet restore ./Services/Identity/src/Identity.API/Identity.API.csproj

# Copy project files
COPY ./src/Shared ./Shared/
COPY ./src/Services/Identity/src/Identity/  ./Services/Identity/src/Identity/
COPY ./src/Services/Identity/src/Identity.API/  ./Services/Identity/src/Identity.API/

# Build project with Release configuration and no restore (already done restore)

RUN ls
RUN dotnet build  -c Release --no-restore ./Services/Identity/src/Identity.API/Identity.API.csproj

WORKDIR /Services/Identity/src/Identity.API

# Publish project to output folder and no build (already done build)
RUN dotnet publish -c Release --no-build -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0-preview

# Setup working directory for the project
WORKDIR /
COPY --from=builder /Services/Identity/src/Identity.API/out  .

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_HTTP_PORT=80
ENV ASPNETCORE_ENVIRONMENT=docker

EXPOSE 80

ENTRYPOINT ["dotnet", "Identity.API.dll"]
