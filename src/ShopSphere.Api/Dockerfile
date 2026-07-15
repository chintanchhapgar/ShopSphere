FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files (better layer caching)
COPY ["src/ShopSphere.Api/ShopSphere.Api.csproj", "src/ShopSphere.Api/"]
COPY ["src/ShopSphere.Application/ShopSphere.Application.csproj", "src/ShopSphere.Application/"]
COPY ["src/ShopSphere.Domain/ShopSphere.Domain.csproj", "src/ShopSphere.Domain/"]
COPY ["src/ShopSphere.Infrastructure/ShopSphere.Infrastructure.csproj", "src/ShopSphere.Infrastructure/"]
COPY ["src/ShopSphere.Contracts/ShopSphere.Contracts.csproj", "src/ShopSphere.Contracts/"]

RUN dotnet restore "src/ShopSphere.Api/ShopSphere.Api.csproj"

# Copy all source code
COPY . .

WORKDIR "/src/src/ShopSphere.Api"
RUN dotnet build "ShopSphere.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShopSphere.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

# Create uploads directory
RUN mkdir -p /app/uploads

EXPOSE 8080
EXPOSE 8081

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "ShopSphere.Api.dll"]