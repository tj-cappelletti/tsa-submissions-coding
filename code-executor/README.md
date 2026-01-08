# TSA Submissions Coding - Code Executor

An event-driven code execution system using RabbitMQ for queuing student submissions and Kubernetes Jobs for isolated, language-specific code execution.

## Architecture Overview

### System Components

1. **Queue Worker** (.NET Worker Service) - Monitors RabbitMQ queue and orchestrates execution
2. **Language Runner** (Self-contained .NET executable) - Executes test cases in isolation
3. **RabbitMQ** - Message queue for submission processing (FIFO order)
4. **Kubernetes** - Container orchestration for isolated code execution
5. **API** - Provides submission details and receives results

### System Flow

```
┌─────────────┐      ┌──────────────┐      ┌─────────────┐
│  RabbitMQ   │─────>│    Worker    │─────>│     API     │
│   Queue     │      │   Service    │      │             │
└─────────────┘      └──────────────┘      └─────────────┘
                            │
                            ├── Fetch submission & test cases
                            │
                            ▼
                     ┌──────────────┐
                     │  Kubernetes  │
                     │     Job      │
                     └──────────────┘
                            │
                            ├── Language-specific container
                            │   (Python, Java, C++, C, Go, Node.js, Ruby, .NET)
                            │
                            ▼
                     ┌──────────────┐
                     │    Runner    │
                     │  Executable  │
                     └──────────────┘
```

### Key Design Decisions

- **All code in .NET**: Both worker and runners use .NET for consistency
- **Self-contained runners**: Runner published as single executable (no .NET runtime in images)
- **Language-specific images**: Each language has its own Docker image
- **Pure execution runners**: Runners only execute code and return results
- **Single source of truth**: All API interactions in worker only

## Supported Languages

- Python 3.13
- Java 21 (OpenJDK LTS)
- C++ (GCC 11.4+)
- C (GCC 11.4+)
- Go 1.23
- Node.js 22 LTS
- Ruby 3.3
- .NET 9.0 (C#, F#, Visual Basic)

## Project Structure

```
code-executor/
├── Tsa.Submissions.Coding.CodeExecutor.sln
├── .editorconfig
├── src/
│   ├── Tsa.Submissions.Coding.CodeExecutor.Shared/
│   │   ├── Models/              # Shared data models
│   │   └── Constants/           # Language constants
│   │
│   ├── Tsa.Submissions.Coding.CodeExecutor.Worker/
│   │   ├── Services/
│   │   │   ├── SubmissionProcessor.cs      # Main orchestration
│   │   │   ├── KubernetesJobManager.cs     # K8s interactions
│   │   │   └── ApiClient.cs                # API communication
│   │   ├── Models/              # Worker-specific models
│   │   ├── Dockerfile           # Worker container
│   │   └── appsettings.json     # Configuration
│   │
│   └── Tsa.Submissions.Coding.CodeExecutor.Runner/
│       ├── Executors/           # Language-specific executors
│       ├── Services/
│       │   └── TestCaseRunner.cs
│       └── Dockerfiles/         # One per language
│
└── tests/
    └── Tsa.Submissions.Coding.CodeExecutor.UnitTests/
```

## Building the Solution

### Prerequisites

- .NET 9.0 SDK
- Docker (for building images)
- Kubernetes cluster (K3s, minikube, or cloud provider)
- RabbitMQ instance

### Build Steps

```bash
# Restore dependencies
cd code-executor
dotnet restore

# Build all projects
dotnet build

# Run tests
dotnet test

# Publish Runner (self-contained)
cd src/Tsa.Submissions.Coding.CodeExecutor.Runner
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true
```

## Building Docker Images

### Language Runner Images

Build all language runner images from the `code-executor` directory:

```bash
# Python
docker build -f src/Tsa.Submissions.Coding.CodeExecutor.Runner/Dockerfiles/Dockerfile.python \
  -t your-registry/python-runner:latest .

# Java
docker build -f src/Tsa.Submissions.Coding.CodeExecutor.Runner/Dockerfiles/Dockerfile.java \
  -t your-registry/java-runner:latest .

# C++
docker build -f src/Tsa.Submissions.Coding.CodeExecutor.Runner/Dockerfiles/Dockerfile.cpp \
  -t your-registry/cpp-runner:latest .

# C
docker build -f src/Tsa.Submissions.Coding.CodeExecutor.Runner/Dockerfiles/Dockerfile.c \
  -t your-registry/c-runner:latest .

# Go
docker build -f src/Tsa.Submissions.Coding.CodeExecutor.Runner/Dockerfiles/Dockerfile.go \
  -t your-registry/go-runner:latest .

# Node.js
docker build -f src/Tsa.Submissions.Coding.CodeExecutor.Runner/Dockerfiles/Dockerfile.nodejs \
  -t your-registry/nodejs-runner:latest .

# Ruby
docker build -f src/Tsa.Submissions.Coding.CodeExecutor.Runner/Dockerfiles/Dockerfile.ruby \
  -t your-registry/ruby-runner:latest .

# .NET
docker build -f src/Tsa.Submissions.Coding.CodeExecutor.Runner/Dockerfiles/Dockerfile.dotnet \
  -t your-registry/dotnet-runner:latest .
```

### Worker Image

```bash
cd code-executor
docker build -f src/Tsa.Submissions.Coding.CodeExecutor.Worker/Dockerfile \
  -t your-registry/code-executor-worker:latest .
```

## Configuration

### Worker Configuration (appsettings.json)

```json
{
  "RabbitMQ": {
    "Host": "rabbitmq-service",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest",
    "QueueName": "code-submissions",
    "DeadLetterQueueName": "code-submissions-dlq"
  },
  "Kubernetes": {
    "Namespace": "code-execution",
    "JobTimeoutMinutes": 3
  },
  "Api": {
    "BaseUrl": "http://api-service:5000"
  },
  "ImageRegistry": {
    "BaseUrl": "your-registry",
    "Images": {
      "Python": "python-runner:latest",
      "Java": "java-runner:latest",
      "CSharp": "dotnet-runner:latest",
      "FSharp": "dotnet-runner:latest",
      "VisualBasic": "dotnet-runner:latest",
      "Cpp": "cpp-runner:latest",
      "C": "c-runner:latest",
      "Go": "go-runner:latest",
      "NodeJs": "nodejs-runner:latest",
      "Ruby": "ruby-runner:latest"
    }
  }
}
```

## Deployment (K3s)

### 1. Create Namespace

```bash
kubectl create namespace code-execution
```

### 2. Deploy RabbitMQ (if not already deployed)

```bash
kubectl apply -f - <<EOF
apiVersion: apps/v1
kind: Deployment
metadata:
  name: rabbitmq
  namespace: code-execution
spec:
  replicas: 1
  selector:
    matchLabels:
      app: rabbitmq
  template:
    metadata:
      labels:
        app: rabbitmq
    spec:
      containers:
      - name: rabbitmq
        image: rabbitmq:3-management
        ports:
        - containerPort: 5672
        - containerPort: 15672
---
apiVersion: v1
kind: Service
metadata:
  name: rabbitmq-service
  namespace: code-execution
spec:
  selector:
    app: rabbitmq
  ports:
  - name: amqp
    port: 5672
  - name: management
    port: 15672
EOF
```

### 3. Deploy Worker

Create `worker-deployment.yaml`:

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: code-executor-worker
  namespace: code-execution
spec:
  replicas: 3
  selector:
    matchLabels:
      app: code-executor-worker
  template:
    metadata:
      labels:
        app: code-executor-worker
    spec:
      serviceAccountName: code-executor-sa
      containers:
      - name: worker
        image: your-registry/code-executor-worker:latest
        imagePullPolicy: IfNotPresent
        env:
        - name: RabbitMQ__Host
          value: "rabbitmq-service"
        - name: Kubernetes__Namespace
          value: "code-execution"
        - name: Api__BaseUrl
          value: "http://api-service:5000"
        - name: ImageRegistry__BaseUrl
          value: "your-registry"
        resources:
          limits:
            memory: "512Mi"
            cpu: "500m"
          requests:
            memory: "256Mi"
            cpu: "250m"
---
apiVersion: v1
kind: ServiceAccount
metadata:
  name: code-executor-sa
  namespace: code-execution
---
apiVersion: rbac.authorization.k8s.io/v1
kind: Role
metadata:
  name: code-executor-role
  namespace: code-execution
rules:
- apiGroups: ["batch"]
  resources: ["jobs"]
  verbs: ["get", "list", "create", "delete"]
- apiGroups: [""]
  resources: ["pods", "pods/log"]
  verbs: ["get", "list"]
---
apiVersion: rbac.authorization.k8s.io/v1
kind: RoleBinding
metadata:
  name: code-executor-binding
  namespace: code-execution
subjects:
- kind: ServiceAccount
  name: code-executor-sa
roleRef:
  kind: Role
  name: code-executor-role
  apiGroup: rbac.authorization.k8s.io
```

Apply the deployment:

```bash
kubectl apply -f worker-deployment.yaml
```

### 4. Pre-load Runner Images (Air-gapped environment)

```bash
# On each K3s node
docker pull your-registry/python-runner:latest
docker pull your-registry/java-runner:latest
docker pull your-registry/cpp-runner:latest
docker pull your-registry/c-runner:latest
docker pull your-registry/go-runner:latest
docker pull your-registry/nodejs-runner:latest
docker pull your-registry/ruby-runner:latest
docker pull your-registry/dotnet-runner:latest

# Import to k3s
k3s ctr images import python-runner.tar
# ... repeat for all images
```

## Security

### Container Security

- **Non-root execution**: All containers run as UID 1000
- **Read-only filesystem**: Where applicable
- **Limited capabilities**: DROP ALL capabilities
- **Resource limits**: CPU and memory constraints
- **Isolated execution**: Each submission runs in its own Kubernetes Job

### Process Isolation

- **Timeouts**: 30s default per test case, 3 minutes per job
- **Resource quotas**: Configurable CPU/memory limits
- **Automatic cleanup**: TTL on jobs (5 minutes)

### Known Issues

- **KubernetesClient vulnerability**: The package has a known moderate severity vulnerability (GHSA-w7r3-mgwf-4mqq). Using the latest available version (17.0.4). Consider updating when a patched version is released.

## Testing

### Unit Tests

```bash
cd code-executor
dotnet test
```

### Integration Testing

For integration testing with actual language runtimes, ensure the required compilers/interpreters are installed:

```bash
# Install prerequisites (Ubuntu/Debian)
sudo apt-get update
sudo apt-get install -y python3 openjdk-21-jdk g++ gcc golang nodejs ruby

# Run tests
dotnet test --filter "TestCategory=UnitTest"
```

## Monitoring & Troubleshooting

### View Worker Logs

```bash
kubectl logs -n code-execution -l app=code-executor-worker -f
```

### View Job Logs

```bash
# List all jobs
kubectl get jobs -n code-execution

# View specific job logs
kubectl logs -n code-execution job/exec-submission-123-456
```

### Common Issues

#### Issue: Jobs not starting

**Solution**: Check image pull policy and ensure images are available

```bash
kubectl describe pod -n code-execution <pod-name>
```

#### Issue: Jobs timing out

**Solution**: Adjust `JobTimeoutMinutes` in configuration

#### Issue: RabbitMQ connection errors

**Solution**: Verify RabbitMQ service is running and accessible

```bash
kubectl get svc -n code-execution rabbitmq-service
```

## Performance Considerations

- **Concurrent workers**: Deploy multiple worker replicas for higher throughput
- **Job TTL**: Automatic cleanup after 5 minutes prevents resource exhaustion
- **Prefetch count**: Set to 1 for fair distribution across workers
- **Resource limits**: Tune based on expected workload

## Future Enhancements

- API endpoints for submission and results management
- Real-time submission status updates via WebSockets
- Enhanced monitoring with Prometheus/Grafana
- Support for additional programming languages
- Custom test frameworks integration
- Code coverage metrics

## Contributing

Please follow the existing code style and conventions defined in `.editorconfig`.

## License

See LICENSE file in the root of the repository.
