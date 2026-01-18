using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Tsa.Submissions.Coding.CodeExecutor.Worker.Configuration;
using Tsa.Submissions.Coding.CodeExecutor.Worker.QueueConsumers;

namespace Tsa.Submissions.Coding.CodeExecutor.Worker.Services;

public class SubmissionJobDispatcher : BackgroundService
{
    private readonly ILogger<SubmissionJobDispatcher> _logger;
    private readonly RabbitMQConfig _rabbitMQConfig;
    private readonly IServiceProvider _serviceProvider;
    private IChannel? _channel;
    private IConnection? _connection;


    public SubmissionJobDispatcher(ILogger<SubmissionJobDispatcher> logger, IOptions<RabbitMQConfig> rabbitMQConfig, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _rabbitMQConfig = rabbitMQConfig.Value;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Submission Job Dispatcher started at: {time}", DateTimeOffset.Now);

        try
        {
            await InitializeRabbitMQAsync(cancellationToken);

            while (!cancellationToken.IsCancellationRequested) await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "A fatal error occurred in Submission Job Dispatcher: {message}", ex.Message);
            throw;
        }
    }

    private async Task InitializeRabbitMQAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Connecting to RabbitMQ at {Host}:{Port}", _rabbitMQConfig.HostName, _rabbitMQConfig.Port);

        var factory = new ConnectionFactory
        {
            HostName = _rabbitMQConfig.HostName,
            Port = int.Parse(_rabbitMQConfig.Port),
            UserName = _rabbitMQConfig.UserName,
            Password = _rabbitMQConfig.Password
        };

        _logger.LogDebug("Attempting to create connection to RabbitMQ");
        _connection = await factory.CreateConnectionAsync(cancellationToken);

        _logger.LogDebug("Connection to RabbitMQ established, creating channel");
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        try
        {
            // Passive declaration - verifies queue exists without modifying it
            _logger.LogDebug("Verifying existence of queue '{QueueName}'", _rabbitMQConfig.QueueName);
            await _channel.QueueDeclarePassiveAsync(_rabbitMQConfig.QueueName, cancellationToken);
            _logger.LogInformation("Successfully verified queue '{QueueName}' exists", _rabbitMQConfig.QueueName);
        }
        catch (OperationInterruptedException ex)
        {
            _logger.LogError("Queue '{QueueName}' does not exist. Please ensure RabbitMQ is configured with the correct definitions.",
                _rabbitMQConfig.QueueName);
            throw new InvalidOperationException($"Required queue '{_rabbitMQConfig.QueueName}' does not exist.", ex);
        }

        // Set prefetch count to 1 for fair distribution
        _logger.LogDebug("Setting prefetch count to 1 for fair message distribution");
        await _channel.BasicQosAsync(0, 1, false, cancellationToken);

        _logger.LogInformation("Connected to RabbitMQ, listening on queue: {QueueName}", _rabbitMQConfig.QueueName);

        using var scope = _serviceProvider.CreateScope();
        var apiClient = scope.ServiceProvider.GetRequiredService<ApiClient>();
        var jobManager = scope.ServiceProvider.GetRequiredService<KubernetesJobManager>();

        var consumer = new SubmissionsQueueConsumer(apiClient, _channel, jobManager, _logger);

        // Start consuming messages
        await _channel.BasicConsumeAsync(
            _rabbitMQConfig.QueueName,
            false,
            consumer,
            cancellationToken);

        _logger.LogInformation("Consumer registered and listening for messages");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Submission Processor stopping...");

        if (_channel != null)
        {
            await _channel.CloseAsync(cancellationToken);
            await _channel.DisposeAsync();
        }

        if (_connection != null)
        {
            await _connection.CloseAsync(cancellationToken);
            await _connection.DisposeAsync();
        }

        await base.StopAsync(cancellationToken);
    }
}
