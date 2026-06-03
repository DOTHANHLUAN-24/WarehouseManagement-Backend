# Base image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# SDK image for compiling the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files and restore to leverage Docker caching
COPY ["src/WarehouseManagement.BackendServer/WarehouseManagement.BackendServer.csproj", "src/WarehouseManagement.BackendServer/"]
COPY ["src/WarehouseManagement.ViewModels/WarehouseManagement.ViewModels.csproj", "src/WarehouseManagement.ViewModels/"]
RUN dotnet restore "src/WarehouseManagement.BackendServer/WarehouseManagement.BackendServer.csproj"

# Copy the rest of the source code and build
COPY . .
WORKDIR "/src/src/WarehouseManagement.BackendServer"
RUN dotnet build "WarehouseManagement.BackendServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WarehouseManagement.BackendServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --chown=app:app --from=publish /app/publish .
USER app
ENTRYPOINT ["dotnet", "WarehouseManagement.BackendServer.dll"]
