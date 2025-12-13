# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files first for better layer caching
COPY CodeMeet.sln .
COPY src/CodeMeet.Api/CodeMeet.Api.csproj src/CodeMeet.Api/
COPY src/CodeMeet.Application/CodeMeet.Application.csproj src/CodeMeet.Application/
COPY src/CodeMeet.Domain/CodeMeet.Domain.csproj src/CodeMeet.Domain/
COPY src/CodeMeet.Infrastructure/CodeMeet.Infrastructure.csproj src/CodeMeet.Infrastructure/
COPY src/CodeMeet.Ddd/CodeMeet.Ddd.csproj src/CodeMeet.Ddd/
COPY tests/CodeMeet.Application.Tests/CodeMeet.Application.Tests.csproj tests/CodeMeet.Application.Tests/
COPY tests/CodeMeet.Domain.Tests/CodeMeet.Domain.Tests.csproj tests/CodeMeet.Domain.Tests/

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY src/ src/
COPY tests/ tests/

# Build and publish
RUN dotnet publish src/CodeMeet.Api/CodeMeet.Api.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN groupadd -r codemeet && useradd -r -g codemeet codemeet

# Create data directory for JSON persistence
RUN mkdir -p /app/data && chown -R codemeet:codemeet /app/data

# Copy published app
COPY --from=build /app/publish .

# Change ownership of app files
RUN chown -R codemeet:codemeet /app

# Switch to non-root user
USER codemeet

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV Persistence__DataDirectory=/app/data

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "CodeMeet.Api.dll"]
