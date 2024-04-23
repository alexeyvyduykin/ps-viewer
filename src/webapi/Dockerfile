#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base-env
WORKDIR /App
EXPOSE 5000

ENV ASPNETCORE_URLS=http://*:5000

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /App
USER appuser

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
ARG configuration=Release

# Shared.Models
WORKDIR /App/shared/Shared.Models
COPY shared/Shared.Models/*.csproj ./

# Shared.Entities.MongoDB
WORKDIR /App/shared/Shared.Entities.MongoDB
COPY shared/Shared.Entities.MongoDB/*.csproj ./

# FlatSpace
WORKDIR /App/libs/FlatSpace
COPY libs/FlatSpace/*.csproj ./

# webapi
WORKDIR /App/src/webapi
COPY src/webapi/*.csproj ./
RUN dotnet restore

# Copy everything
WORKDIR /App/shared/Shared.Models
COPY shared/Shared.Models/ ./ 

WORKDIR /App/shared/Shared.Entities.MongoDB
COPY shared/Shared.Entities.MongoDB/ ./

WORKDIR /App/libs/FlatSpace
COPY libs/FlatSpace/ ./

WORKDIR /App/src/webapi
COPY src/webapi/ ./

# Build
RUN dotnet build "webapi.csproj" -c $configuration -o build

FROM build-env AS publish
ARG configuration=Release
# Publish
RUN dotnet publish "webapi.csproj" -c $configuration -o publish /p:UseAppHost=false

FROM base-env AS final
WORKDIR /App
COPY --from=publish /App/src/webapi/publish .
ENTRYPOINT ["dotnet", "webapi.dll"]

# Create Image
# docker build --tag webapi-img -f src/webapi/Dockerfile .

# Create Container
# docker run --rm -d -p 5000:5000/tcp webapi-img