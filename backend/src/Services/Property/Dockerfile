FROM mcr.microsoft.com/dotnet/sdk:8.0-preview AS builder

# Setup working directory for the project
WORKDIR /
COPY ./src/Shared/Shared.csproj ./Shared/
COPY ./src/Services/Property/src/Property/Property.csproj ./Services/Property/src/Property/
COPY ./src/Services/Property/src/Property.API/Property.API.csproj ./Services/Property/src/Property.API/

# Restore nuget packages
RUN dotnet restore ./Services/Property/src/Property.API/Property.API.csproj

# Copy project files
COPY ./src/Shared ./Shared/
COPY ./src/Services/Property/src/Property/  ./Services/Property/src/Property/
COPY ./src/Services/Property/src/Property.API/  ./Services/Property/src/Property.API/

# Build project with Release configuration and no restore (already done restore)

RUN ls
RUN dotnet build  -c Release --no-restore ./Services/Property/src/Property.API/Property.API.csproj

WORKDIR /Services/Property/src/Property.API

# Publish project to output folder and no build (already done build)
RUN dotnet publish -c Release --no-build -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0-preview

# Setup working directory for the project
WORKDIR /
COPY --from=builder /Services/Property/src/Property.API/out  .

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_HTTP_PORT=80
ENV ASPNETCORE_ENVIRONMENT=docker

EXPOSE 80

ENTRYPOINT ["dotnet", "Property.API.dll"]