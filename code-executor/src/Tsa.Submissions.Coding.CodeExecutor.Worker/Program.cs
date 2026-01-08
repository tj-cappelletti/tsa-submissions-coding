using k8s;
using Tsa.Submissions.Coding.CodeExecutor.Worker.Services;

var builder = Host.CreateApplicationBuilder(args);

// Configure Kubernetes client
builder.Services.AddSingleton<IKubernetes>(sp =>
{
    var config = KubernetesClientConfiguration.IsInCluster()
        ? KubernetesClientConfiguration.InClusterConfig()
        : KubernetesClientConfiguration.BuildConfigFromConfigFile();
    
    return new Kubernetes(config);
});

// Configure HTTP client for API
builder.Services.AddHttpClient<ApiClient>((sp, client) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["Api:BaseUrl"] ?? "http://localhost:5000";
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Register services
builder.Services.AddScoped<KubernetesJobManager>();
builder.Services.AddHostedService<SubmissionProcessor>();

var host = builder.Build();
host.Run();
