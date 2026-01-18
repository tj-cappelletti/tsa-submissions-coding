# Global ARG for Java version
ARG LANG_VERSION=21

# Build Stage - Code Executor Runner
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the project files and restore dependencies
COPY ["Tsa.Submissions.Coding.CodeExecutor.Runner/Tsa.Submissions.Coding.CodeExecutor.Runner.csproj", "Tsa.Submissions.Coding.CodeExecutor.Runner/"]
COPY ["Tsa.Submissions.Coding.CodeExecutor.Shared/Tsa.Submissions.Coding.CodeExecutor.Shared.csproj", "Tsa.Submissions.Coding.CodeExecutor.Shared/"]

# Restore dependencies
RUN dotnet restore "Tsa.Submissions.Coding.CodeExecutor.Runner/Tsa.Submissions.Coding.CodeExecutor.Runner.csproj"

# Copy source code
COPY ["Tsa.Submissions.Coding.CodeExecutor.Shared/", "Tsa.Submissions.Coding.CodeExecutor.Shared/"]
COPY ["Tsa.Submissions.Coding.CodeExecutor.Runner/", "Tsa.Submissions.Coding.CodeExecutor.Runner/"]

# Build and publish as self-contained executable
WORKDIR /src/Tsa.Submissions.Coding.CodeExecutor.Runner
RUN dotnet publish -c Release -o /app/publish --self-contained true -r linux-x64 /p:PublishSingleFile=true

# Final Stage - Runtime
FROM eclipse-temurin:${LANG_VERSION}-jdk-jammy

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
