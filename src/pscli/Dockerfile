FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App/src/pscli

#
WORKDIR /App/shared/Shared.Models
COPY shared/Shared.Models/*.csproj ./
RUN dotnet restore

# 
WORKDIR /App/libs/FlatSpace
COPY libs/FlatSpace/*.csproj ./
RUN dotnet restore

# 
WORKDIR /App/src/pscli
COPY src/pscli/*.csproj ./
RUN dotnet restore

# Copy everything
WORKDIR /App/shared/Shared.Models
COPY shared/Shared.Models/ ./

WORKDIR /App/libs/FlatSpace
COPY libs/FlatSpace/ ./

WORKDIR /App/src/pscli
COPY src/pscli/ ./

# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /App
COPY --from=build-env /App/src/pscli/out .
ENTRYPOINT ["dotnet", "pscli.dll"]