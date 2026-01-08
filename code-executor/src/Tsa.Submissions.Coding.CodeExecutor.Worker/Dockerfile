# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/Tsa.Submissions.Coding.CodeExecutor.Shared/Tsa.Submissions.Coding.CodeExecutor.Shared.csproj", "Shared/"]
COPY ["src/Tsa.Submissions.Coding.CodeExecutor.Worker/Tsa.Submissions.Coding.CodeExecutor.Worker.csproj", "Worker/"]

# Restore dependencies
RUN dotnet restore "Worker/Tsa.Submissions.Coding.CodeExecutor.Worker.csproj"

# Copy source code
COPY ["src/Tsa.Submissions.Coding.CodeExecutor.Shared/", "Shared/"]
COPY ["src/Tsa.Submissions.Coding.CodeExecutor.Worker/", "Worker/"]

# Build and publish
WORKDIR /src/Worker
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Create non-root user
RUN useradd -m -u 1000 -s /bin/bash worker

# Copy published application
COPY --from=build /app/publish .

# Set ownership
RUN chown -R worker:worker /app

# Switch to non-root user
USER worker

# Entry point
ENTRYPOINT ["dotnet", "Tsa.Submissions.Coding.CodeExecutor.Worker.dll"]
