FROM alpine AS base
USER $APP_UID
WORKDIR /app

expose 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
RUN apk add clang binutils musl-dev build-base zlib-static
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FilesHashApi/FilesHashApi.csproj", "FilesHashApi/"]
RUN dotnet restore -r linux-musl-x64 "FilesHashApi/FilesHashApi.csproj"
COPY . .
WORKDIR "/src/FilesHashApi"
RUN dotnet build "FilesHashApi.csproj" -c $BUILD_CONFIGURATION -r linux-musl-x64 -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FilesHashApi.csproj" -c $BUILD_CONFIGURATION -r linux-musl-x64 -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["./FilesHashApi"]
