FROM ghcr.io/osgeo/gdal:ubuntu-full-3.8.5 as base
WORKDIR /app
EXPOSE 8080

# The GDAL base image ships an Apache Arrow apt repo whose GPG signing key
# rotates and breaks `apt-get update`. We don't use it (Arrow is already in the
# image), so remove it before updating.
RUN rm -f /etc/apt/sources.list.d/apache-arrow.sources && \
    apt-get update && \
    apt-get install -y wget

RUN wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN rm packages-microsoft-prod.deb

RUN apt-get update && \
    apt-get install software-properties-common -y

RUN add-apt-repository ppa:dotnet/backports

RUN apt-get update && \
    apt-get install -y aspnetcore-runtime-10.0

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src
COPY ["WADNR.GDALAPI/WADNR.GDALAPI.csproj", "WADNR.GDALAPI/"]
RUN dotnet restore "WADNR.GDALAPI/WADNR.GDALAPI.csproj"
COPY WADNR.GDALAPI/ WADNR.GDALAPI/
WORKDIR "/src/WADNR.GDALAPI"
RUN dotnet build "WADNR.GDALAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WADNR.GDALAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WADNR.GDALAPI.dll"]
