FROM mcr.microsoft.com/dotnet/sdk:8.0-preview AS builder

# Setup working directory for the project
WORKDIR /
COPY ./src/Shared/Shared.csproj ./Shared/
COPY ./src/Services/User/src/User/User.csproj ./Services/User/src/User/
COPY ./src/Services/User/src/User.API/User.API.csproj ./Services/User/src/User.API/

# Restore nuget packages
RUN dotnet restore ./Services/User/src/User.API/User.API.csproj

# Copy project files
COPY ./src/Shared ./Shared/
COPY ./src/Services/User/src/User/  ./Services/User/src/User/
COPY ./src/Services/User/src/User.API/  ./Services/User/src/User.API/

# Build project with Release configuration and no restore (already done restore)

RUN ls
RUN dotnet build  -c Release --no-restore ./Services/User/src/User.API/User.API.csproj

WORKDIR /Services/User/src/User.API

# Publish project to output folder and no build (already done build)
RUN dotnet publish -c Release --no-build -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0-preview

# Setup working directory for the project
WORKDIR /
COPY --from=builder /Services/User/src/User.API/out  .

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_HTTP_PORT=80
ENV ASPNETCORE_ENVIRONMENT=docker

EXPOSE 80

ENTRYPOINT ["dotnet", "User.API.dll"]