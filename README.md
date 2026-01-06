# TSA Submissions Coding

A platform to host and manage the TSA Coding challenge for regional and state competitions.

## Architecture

- **Frontend**: Angular 21 (TypeScript)
- **Backend**: .NET 10 Web API (C#)
- **Containerization**: Docker

## Project Structure

```
├── src/
│   └── Tsa.Submissions.Coding.WebApi/   # .NET 10 backend API
│       ├── Program.cs                   # API entry point
│       ├── Dockerfile                   # Docker build configuration
│       └── *.csproj                     # Project file
├── tests/
│   └── Tsa.Submissions.Coding.UnitTests/ # xUnit test project
│       └── *.csproj                      # Test project file
├── ui/                                   # Angular 21 frontend application
│   ├── src/                             # Source files
│   ├── Dockerfile                       # Docker build configuration
│   └── nginx.conf                       # Nginx configuration for production
├── Tsa.Submissions.Coding.sln           # Solution file
└── .github/
    └── workflows/                       # CI/CD workflows
        ├── ui-build.yml                 # UI build and deployment
        └── api-build.yml                # API build and deployment
```

## Development

### Prerequisites

- Node.js 20.x or higher
- .NET 10 SDK
- Docker or Podman (for containerization)

### UI Development

```bash
cd ui
npm install
npm start
```

The UI will be available at `http://localhost:4200`

### API Development

```bash
cd src/Tsa.Submissions.Coding.WebApi
dotnet restore
dotnet run
```

The API will be available at `http://localhost:5000` (HTTP) and `https://localhost:5001` (HTTPS)

## Building

### Build Solution

```bash
dotnet build Tsa.Submissions.Coding.sln --configuration Release
```

### Run Tests

```bash
dotnet test Tsa.Submissions.Coding.sln --configuration Release
```

### UI Build

```bash
cd ui
npm run build
```

Build output will be in `ui/dist/ui/browser/`

### API Build and Publish

```bash
dotnet build src/Tsa.Submissions.Coding.WebApi/Tsa.Submissions.Coding.WebApi.csproj --configuration Release
dotnet publish src/Tsa.Submissions.Coding.WebApi/Tsa.Submissions.Coding.WebApi.csproj --configuration Release --output ./publish
```

Publish output will be in `./publish/`

## Docker Images

Both UI and API have Dockerfile configurations for containerization.

> **Note**: All Docker commands below work with both Docker and Podman. Simply replace `docker` with `podman` or `docker-compose` with `podman compose` as needed.

### Build UI Docker Image

```bash
cd ui
docker build -t tsa-ui:latest .
# OR with Podman
podman build -t tsa-ui:latest .
```

### Build API Docker Image

```bash
cd src/Tsa.Submissions.Coding.WebApi
docker build -t tsa-api:latest .
# OR with Podman
podman build -t tsa-api:latest .
```

### Run with Docker Compose

The easiest way to run both services together is using Docker Compose:

```bash
docker-compose up --build
# OR with Podman (4.0+)
podman compose up --build
```

This will:
- Build both the UI and API Docker images
- Start both services
- UI will be available at `http://localhost:4200`
- API will be available at `http://localhost:5000`

To stop the services:

```bash
docker-compose down
# OR with Podman
podman compose down
```

## CI/CD

The project includes GitHub Actions workflows that automatically:

1. Build the applications
2. Run tests
3. Create build artifacts
4. Build and push Docker images to GitHub Container Registry

Workflows are triggered on:
- Push to `main` or `develop` branches
- Pull requests to `main` or `develop` branches
- Manual workflow dispatch

### Artifacts

- **UI artifacts**: Built Angular application ready for deployment
- **API artifacts**: Published .NET application ready for deployment

### Container Images

Container images are automatically pushed to GitHub Container Registry (ghcr.io) with tags:
- `latest` (for main branch)
- Branch name
- Git SHA

## License

See [LICENSE](LICENSE) file for details.
