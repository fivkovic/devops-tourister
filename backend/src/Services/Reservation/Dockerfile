FROM mcr.microsoft.com/dotnet/sdk:8.0-preview AS builder

# Setup working directory for the project
WORKDIR /
COPY ./src/Shared/Shared.csproj ./Shared/
COPY ./src/Services/Reservation/src/Reservation/Reservation.csproj ./Services/Reservation/src/Reservation/
COPY ./src/Services/Reservation/src/Reservation.API/Reservation.API.csproj ./Services/Reservation/src/Reservation.API/

# Restore nuget packages
RUN dotnet restore ./Services/Reservation/src/Reservation.API/Reservation.API.csproj

# Copy project files
COPY ./src/Shared ./Shared/
COPY ./src/Services/Reservation/src/Reservation/  ./Services/Reservation/src/Reservation/
COPY ./src/Services/Reservation/src/Reservation.API/  ./Services/Reservation/src/Reservation.API/

# Build project with Release configuration and no restore (already done restore)

RUN ls
RUN dotnet build  -c Release --no-restore ./Services/Reservation/src/Reservation.API/Reservation.API.csproj

WORKDIR /Services/Reservation/src/Reservation.API

# Publish project to output folder and no build (already done build)
RUN dotnet publish -c Release --no-build -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0-preview

# Setup working directory for the project
WORKDIR /
COPY --from=builder /Services/Reservation/src/Reservation.API/out  .

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_HTTP_PORT=80
ENV ASPNETCORE_ENVIRONMENT=docker

EXPOSE 80

ENTRYPOINT ["dotnet", "Reservation.API.dll"]