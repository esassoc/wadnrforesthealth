#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WADNRForestHealthTracker.API/WADNRForestHealthTracker.API.csproj", "WADNRForestHealthTracker.API/"]

RUN dotnet restore "./WADNRForestHealthTracker.API/./WADNRForestHealthTracker.API.csproj"
COPY . .
WORKDIR "/src/WADNRForestHealthTracker.API"
RUN dotnet build "./WADNRForestHealthTracker.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WADNRForestHealthTracker.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WADNRForestHealthTracker.API.dll"]