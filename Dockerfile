FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
USER $APP_UID
WORKDIR /app

expose 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
RUN ls -la
COPY ["FilesHashApi/FilesHashApi.csproj", "FilesHashApi/"]
RUN dotnet restore "FilesHashApi/FilesHashApi.csproj"
COPY . .
WORKDIR "/src/FilesHashApi"
RUN dotnet build "FilesHashApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FilesHashApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FilesHashApi.dll"]
