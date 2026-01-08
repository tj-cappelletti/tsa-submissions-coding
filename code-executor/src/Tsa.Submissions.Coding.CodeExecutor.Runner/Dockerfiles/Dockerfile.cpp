# Build stage - Build the .NET runner
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/Tsa.Submissions.Coding.CodeExecutor.Shared/Tsa.Submissions.Coding.CodeExecutor.Shared.csproj", "Shared/"]
COPY ["src/Tsa.Submissions.Coding.CodeExecutor.Runner/Tsa.Submissions.Coding.CodeExecutor.Runner.csproj", "Runner/"]

# Restore dependencies
RUN dotnet restore "Runner/Tsa.Submissions.Coding.CodeExecutor.Runner.csproj"

# Copy source code
COPY ["src/Tsa.Submissions.Coding.CodeExecutor.Shared/", "Shared/"]
COPY ["src/Tsa.Submissions.Coding.CodeExecutor.Runner/", "Runner/"]

# Build and publish as self-contained executable
WORKDIR /src/Runner
RUN dotnet publish -c Release -o /app/publish --self-contained true -r linux-x64 /p:PublishSingleFile=true

# Runtime stage - C++
FROM gcc:11.4

# Create non-root user
RUN useradd -m -u 1000 -s /bin/bash coderunner

# Copy the runner executable
COPY --from=build /app/publish/Tsa.Submissions.Coding.CodeExecutor.Runner /usr/local/bin/runner
RUN chmod +x /usr/local/bin/runner

# Set working directory
WORKDIR /workspace
RUN chown coderunner:coderunner /workspace

# Switch to non-root user
USER coderunner

# Entry point
ENTRYPOINT ["/usr/local/bin/runner"]
