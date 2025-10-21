# ============================================
# Stage 1: Build
# ============================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY ["LegacyProcs.sln", "./"]

# Copy project files
COPY ["LegacyProcs/LegacyProcs.csproj", "LegacyProcs/"]
COPY ["LegacyProcs.Tests/LegacyProcs.Tests.csproj", "LegacyProcs.Tests/"]

# Restore dependencies
RUN dotnet restore "LegacyProcs.sln"

# Copy all source files
COPY . .

# Build the application
WORKDIR "/src/LegacyProcs"
RUN dotnet build "LegacyProcs.csproj" -c Release -o /app/build

# ============================================
# Stage 2: Publish
# ============================================
FROM build AS publish
RUN dotnet publish "LegacyProcs.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# ============================================
# Stage 3: Runtime
# ============================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && \
    apt-get install -y curl && \
    rm -rf /var/lib/apt/lists/*

# Create non-root user
RUN groupadd -r appuser && \
    useradd -r -g appuser appuser && \
    chown -R appuser:appuser /app

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Copy published files
COPY --from=publish --chown=appuser:appuser /app/publish .

# Switch to non-root user
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Entry point
ENTRYPOINT ["dotnet", "LegacyProcs.dll"]
