FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore EcfDotnet.csproj --disable-parallel

COPY . ./
RUN dotnet build EcfDotnet.csproj -c Release

FROM build AS publish
RUN dotnet publish EcfDotnet.csproj -c Release -o /app/publish --no-build

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5000

ENTRYPOINT ["dotnet", "EcfDotnet.dll"]
