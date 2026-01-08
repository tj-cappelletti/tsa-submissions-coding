using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Tsa.Submissions.Coding.CodeExecutor.Worker.Models;

namespace Tsa.Submissions.Coding.CodeExecutor.Worker.Services;

/// <summary>
/// Background service that processes code submissions from RabbitMQ
/// </summary>
public class SubmissionProcessor : BackgroundService
{
    private readonly ILogger<SubmissionProcessor> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private IConnection? _connection;
    private IModel? _channel;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubmissionProcessor"/> class
    /// </summary>
    public SubmissionProcessor(
        ILogger<SubmissionProcessor> logger,
        IConfiguration configuration,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Executes the background service
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Submission Processor starting...");

        try
        {
            await InitializeRabbitMQAsync(stoppingToken);

            // Keep the service running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Fatal error in Submission Processor");
            throw;
        }
    }

    private Task InitializeRabbitMQAsync(CancellationToken stoppingToken)
    {
        var host = _configuration["RabbitMQ:Host"] ?? "localhost";
        var port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672");
        var username = _configuration["RabbitMQ:Username"] ?? "guest";
        var password = _configuration["RabbitMQ:Password"] ?? "guest";
        var queueName = _configuration["RabbitMQ:QueueName"] ?? "code-submissions";

        _logger.LogInformation("Connecting to RabbitMQ at {Host}:{Port}", host, port);

        var factory = new ConnectionFactory
        {
            HostName = host,
            Port = port,
            UserName = username,
            Password = password,
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Declare queue with dead letter exchange
        var arguments = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", "" },
            { "x-dead-letter-routing-key", _configuration["RabbitMQ:DeadLetterQueueName"] ?? "code-submissions-dlq" }
        };

        _channel.QueueDeclare(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: arguments);

        // Set prefetch count to 1 for fair distribution
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        _logger.LogInformation("Connected to RabbitMQ, listening on queue: {QueueName}", queueName);

        // Create consumer
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            await HandleMessageAsync(ea, stoppingToken);
        };

        _channel.BasicConsume(
            queue: queueName,
            autoAck: false,
            consumer: consumer);
        
        return Task.CompletedTask;
    }

    private async Task HandleMessageAsync(BasicDeliverEventArgs ea, CancellationToken stoppingToken)
    {
        var body = ea.Body.ToArray();
        var messageJson = Encoding.UTF8.GetString(body);

        try
        {
            _logger.LogInformation("Received message: {Message}", messageJson);

            var message = JsonSerializer.Deserialize<SubmissionMessage>(messageJson);
            
            if (message == null)
            {
                _logger.LogError("Failed to deserialize message");
                _channel!.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
                return;
            }

            // Process the submission
            var success = await ProcessSubmissionAsync(message, stoppingToken);

            if (success)
            {
                // Acknowledge message
                _channel!.BasicAck(ea.DeliveryTag, multiple: false);
                _logger.LogInformation("Message processed successfully for submission {SubmissionId}", message.SubmissionId);
            }
            else
            {
                // Reject message - will go to dead letter queue
                _channel!.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
                _logger.LogError("Message processing failed for submission {SubmissionId}", message.SubmissionId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling message");
            
            // Reject message on error
            try
            {
                _channel!.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
            }
            catch
            {
                // Ignore errors during NACK
            }
        }
    }

    private async Task<bool> ProcessSubmissionAsync(SubmissionMessage message, CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var apiClient = scope.ServiceProvider.GetRequiredService<ApiClient>();
        var jobManager = scope.ServiceProvider.GetRequiredService<KubernetesJobManager>();

        try
        {
            _logger.LogInformation("Processing submission {SubmissionId}", message.SubmissionId);

            // Fetch submission details from API
            var payload = await apiClient.GetSubmissionAsync(message.SubmissionId, stoppingToken);
            
            if (payload == null)
            {
                _logger.LogError("Failed to fetch submission {SubmissionId} from API", message.SubmissionId);
                return false;
            }

            // Execute code in Kubernetes Job
            var result = await jobManager.ExecuteJobAsync(payload, stoppingToken);
            
            if (result == null)
            {
                _logger.LogError("Failed to execute job for submission {SubmissionId}", message.SubmissionId);
                return false;
            }

            // Update API with results
            var updated = await apiClient.UpdateSubmissionResultsAsync(result, stoppingToken);
            
            if (!updated)
            {
                _logger.LogError("Failed to update results for submission {SubmissionId}", message.SubmissionId);
                return false;
            }

            _logger.LogInformation("Successfully processed submission {SubmissionId}", message.SubmissionId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing submission {SubmissionId}", message.SubmissionId);
            return false;
        }
    }

    /// <summary>
    /// Cleanup resources when service stops
    /// </summary>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Submission Processor stopping...");

        if (_channel != null)
        {
            _channel.Close();
            _channel.Dispose();
        }

        if (_connection != null)
        {
            _connection.Close();
            _connection.Dispose();
        }

        await base.StopAsync(cancellationToken);
    }
}
