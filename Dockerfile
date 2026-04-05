# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first for better layer caching
COPY StudentManagementSystem.sln ./
COPY StudentManagementSystem.API/StudentManagementSystem.API.csproj StudentManagementSystem.API/
COPY StudentManagementSystem.Core/StudentManagementSystem.Core.csproj StudentManagementSystem.Core/
COPY StudentManagementSystem.Infrastructure/StudentManagementSystem.Infrastructure.csproj StudentManagementSystem.Infrastructure/

# Restore dependencies
RUN dotnet restore StudentManagementSystem.API/StudentManagementSystem.API.csproj

# Copy the rest of the source code
COPY . .

# Build the application
RUN dotnet build StudentManagementSystem.API/StudentManagementSystem.API.csproj -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish StudentManagementSystem.API/StudentManagementSystem.API.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# Copy published files
COPY --from=publish /app/publish .

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:8080/swagger/v1/swagger.json || exit 1

ENTRYPOINT ["dotnet", "StudentManagementSystem.API.dll"]
