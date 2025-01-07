FROM mcr.microsoft.com/dotnet/aspnet:8.0.3-bookworm-slim-amd64 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["IMS/Server/IMS.Server.csproj", "IMS/Server/"]
COPY ["IMS/Client/IMS.Client.csproj", "IMS/Client/"]
COPY ["IMS/Shared/IMS.Shared.csproj", "IMS/Shared/"]
#RUN dotnet restore "IMS/Server/IMS.Server.csproj"
COPY . .
WORKDIR "/src/IMS/Server"
RUN dotnet build "IMS.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "IMS.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IMS.Server.dll"]
