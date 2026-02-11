using k8s;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tsa.Submissions.Coding.ApiClient;
using Tsa.Submissions.Coding.CodeExecutor.Worker.Configuration;
using Tsa.Submissions.Coding.CodeExecutor.Worker.Services;

namespace Tsa.Submissions.Coding.CodeExecutor.Worker;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        using var loggerFactory = LoggerFactory.Create(loggingBuilder =>
        {
            loggingBuilder.AddConfiguration(builder.Configuration.GetSection("Logging"));
            loggingBuilder.AddConsole();
        });

        var logger = loggerFactory.CreateLogger<Program>();

        builder.Services.Configure<KubernetesCluster>(builder.Configuration.GetSection(KubernetesCluster.SectionName));
        builder.Services.Configure<RabbitMQConfig>(builder.Configuration.GetSection(RabbitMQConfig.SectionName));
        builder.Services.Configure<RunnerImageRegistry>(builder.Configuration.GetSection(RunnerImageRegistry.SectionName));
        builder.Services.Configure<SubmissionsApiConfig>(builder.Configuration.GetSection(SubmissionsApiConfig.SectionName));

        builder.Services.AddOptions<SubmissionsApiConfig>()
            .Bind(builder.Configuration.GetSection(SubmissionsApiConfig.SectionName))
            .Validate(config =>
            {
                if (string.IsNullOrWhiteSpace(config.BaseUrl))
                {
                    return false;
                }

                if (!Uri.TryCreate(config.BaseUrl, UriKind.Absolute, out _))
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(config.Authentication.Username))
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(config.Authentication.Password))
                {
                    return false;
                }

                return true;
            }, "SubmissionsApiConfig is invalid. Check BaseUrl, Username, and Password.")
            .ValidateOnStart();

        builder.Services.AddSingleton<ICodingApiClient>(serviceProvider =>
        {
            var config = serviceProvider.GetRequiredService<IOptions<SubmissionsApiConfig>>().Value;

            var uri = new Uri(config.BaseUrl);

            return new CodingApiClient(uri, config.Authentication.Username, config.Authentication.Password);
        });

        builder.Services.AddSingleton<IKubernetes>(serviceProvider =>
        {
            if (KubernetesClientConfiguration.IsInCluster())
            {
                logger.LogInformation("Running in Kubernetes cluster - using InClusterConfig");
                return new Kubernetes(KubernetesClientConfiguration.InClusterConfig());
            }

            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var kubeConfigFile = configuration.GetValue<string>("Kubernetes:KubeConfigFile");

            logger.LogInformation("Running outside Kubernetes cluster - using kubeconfig file..");

            if (kubeConfigFile == null)
            {
                logger.LogWarning("Using kubeconfig at default location");
            }

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
