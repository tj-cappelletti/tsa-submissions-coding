# Global ARG for Java version
ARG LANG_VERSION=21

# Build Stage - Code Executor Runner
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the project files and restore dependencies
COPY ["shared-components/Tsa.Submissions.Coding.Contracts/Tsa.Submissions.Coding.Contracts.csproj", "shared-components/Tsa.Submissions.Coding.Contracts/Tsa.Submissions.Coding.Contracts/"]
COPY ["code-executor/Tsa.Submissions.Coding.CodeExecutor.Runner/Tsa.Submissions.Coding.CodeExecutor.Runner.csproj", "code-executor/Tsa.Submissions.Coding.CodeExecutor.Runner/"]

# Restore dependencies
RUN dotnet restore "code-executor/Tsa.Submissions.Coding.CodeExecutor.Runner/Tsa.Submissions.Coding.CodeExecutor.Runner.csproj"

# Copy source code
COPY ["shared-components/Tsa.Submissions.Coding.Contracts", "shared-components/Tsa.Submissions.Coding.Contracts/"]
COPY ["code-executor/Tsa.Submissions.Coding.CodeExecutor.Runner", "code-executor/Tsa.Submissions.Coding.CodeExecutor.Runner/"]

# Build and publish as self-contained executable
WORKDIR /src/code-executor/Tsa.Submissions.Coding.CodeExecutor.Runner
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
