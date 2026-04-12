FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

# Build Stage - Scorer Executor Runner
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project files and restore dependencies
COPY ["code-executor/Tsa.Submissions.Coding.CodeExecutor.Scorer/Tsa.Submissions.Coding.CodeExecutor.Scorer.csproj", "code-executor/Tsa.Submissions.Coding.CodeExecutor.Scorer/"]
COPY ["shared-components/Tsa.Submissions.Coding.CodeExecutor.Core/Tsa.Submissions.Coding.CodeExecutor.Core.csproj", "shared-components/Tsa.Submissions.Coding.CodeExecutor.Core/"]
COPY ["shared-components/Tsa.Submissions.Coding.Contracts/Tsa.Submissions.Coding.Contracts.csproj", "shared-components/Tsa.Submissions.Coding.Contracts/"]

# Restore dependencies
RUN dotnet restore "./code-executor/Tsa.Submissions.Coding.CodeExecutor.Scorer/Tsa.Submissions.Coding.CodeExecutor.Scorer.csproj"

# Copy source code
COPY ["shared-components/Tsa.Submissions.Coding.CodeExecutor.Core", "shared-components/Tsa.Submissions.Coding.CodeExecutor.Core/"]
COPY ["shared-components/Tsa.Submissions.Coding.Contracts", "shared-components/Tsa.Submissions.Coding.Contracts/"]
COPY ["code-executor/Tsa.Submissions.Coding.CodeExecutor.Scorer", "code-executor/Tsa.Submissions.Coding.CodeExecutor.Scorer/"]

# Build and publish as self-contained executable
WORKDIR /src/code-executor/Tsa.Submissions.Coding.CodeExecutor.Scorer
RUN dotnet publish -c Release -o /app/publish --self-contained true -r linux-x64 /p:PublishSingleFile=true

# Final Stage - Runtime
FROM python:3

# Install lizard globally as root before creating non-root user
RUN pip install lizard

# Create non-root user
RUN useradd -m -s /bin/bash coderunner

# Copy the runner executable
COPY --from=build /app/publish/Tsa.Submissions.Coding.CodeExecutor.Scorer /usr/local/bin/scorer
RUN chmod +x /usr/local/bin/scorer

# Set working directory
WORKDIR /workspace
RUN chown coderunner:coderunner /workspace

# Switch to non-root user
USER coderunner

ENTRYPOINT ["/usr/local/bin/scorer"]