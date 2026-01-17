using k8s;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Tsa.Submissions.Coding.CodeExecutor.Worker.Configuration;
using Tsa.Submissions.Coding.CodeExecutor.Worker.Handlers;
using Tsa.Submissions.Coding.CodeExecutor.Worker.Services;

namespace Tsa.Submissions.Coding.CodeExecutor.Worker;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        using var loggerFactory = LoggerFactory.Create(configure: loggingBuilder =>
        {
            loggingBuilder.AddConfiguration(builder.Configuration.GetSection("Logging"));
            loggingBuilder.AddConsole();
        });

        var logger = loggerFactory.CreateLogger<Program>();

        builder.Services.Configure<RabbitMQConfig>(builder.Configuration.GetSection(RabbitMQConfig.SectionName));
        builder.Services.Configure<SubmissionsApiConfig>(builder.Configuration.GetSection(SubmissionsApiConfig.SectionName));

        builder.Services.AddOptions<SubmissionsApiConfig>()
            .Bind(builder.Configuration.GetSection(SubmissionsApiConfig.SectionName))
            .Validate(validation: config =>
            {
                if (string.IsNullOrWhiteSpace(config.BaseUrl))
                    return false;
                if (!Uri.TryCreate(config.BaseUrl, UriKind.Absolute, out _))
                    return false;
                if (string.IsNullOrWhiteSpace(config.Authentication.Username))
                    return false;
                if (string.IsNullOrWhiteSpace(config.Authentication.Password))
                    return false;
                return true;
            }, "SubmissionsApiConfig is invalid. Check BaseUrl, Username, and Password.")
            .ValidateOnStart();

        // Register the authentication handler
        builder.Services.AddTransient<AuthenticationHandler>();

        builder.Services.AddHttpClient<ApiClient>(configureClient: client =>
            {
                var apiBaseUrl = builder.Configuration["SubmissionsApi:BaseUrl"];

                if (apiBaseUrl == null)
                {
                    logger.LogError("SubmissionsApi:BaseUrl configuration is missing");
                    throw new InvalidOperationException("SubmissionsApi:BaseUrl configuration is missing");
                }

                client.BaseAddress = new Uri(apiBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<AuthenticationHandler>() // Add authentication handler
            .AddStandardResilienceHandler(configure: options =>
            {
                // Configure retry policy for transient failures
                options.Retry.MaxRetryAttempts = 3;
                options.Retry.Delay = TimeSpan.FromSeconds(2);
                options.Retry.BackoffType = DelayBackoffType.Exponential;
            });

        builder.Services.AddSingleton<IKubernetes>(implementationFactory: serviceProvider =>
        {
            if (KubernetesClientConfiguration.IsInCluster())
            {
                logger.LogInformation("Running in Kubernetes cluster - using InClusterConfig");
                return new Kubernetes(KubernetesClientConfiguration.InClusterConfig());
            }

            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var kubeConfigFile = configuration.GetValue<string>("Kubernetes:KubeConfigFile");
            logger.LogInformation("Running outside Kubernetes cluster - using kubeconfig file: {KubeConfigFile}", kubeConfigFile);

            return new Kubernetes(KubernetesClientConfiguration.BuildConfigFromConfigFile(kubeConfigFile));
        });

        builder.Services.AddHostedService<SubmissionJobDispatcher>();
        builder.Services.AddScoped<KubernetesJobManager>();

        logger.LogInformation("Application starting up");

        var app = builder.Build();

        logger.LogInformation("Application host built successfully, running...");

        app.Run();
    }
}
