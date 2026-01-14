using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Tsa.Submissions.Coding.WebApi.Messages;

namespace Tsa.Submissions.Coding.WebApi.Services;

public class RabbitMQService : ISubmissionsQueueService, IDisposable
{
    private readonly IChannel _channel;
    private readonly RabbitMQConfig _rabbitMQConfig;
    private readonly IConnection _connection;
    private readonly ILogger<RabbitMQService> _logger;

    private bool _disposed;

    public RabbitMQService(IOptions<RabbitMQConfig> options, ILogger<RabbitMQService> logger)
    {
        _rabbitMQConfig = options.Value;
        _logger = logger;

        _logger.LogInformation("Connecting to RabbitMQ at {Host}:{Port}", _rabbitMQConfig.HostName, _rabbitMQConfig.Port);

        var factory = new ConnectionFactory
        {
            HostName = _rabbitMQConfig.HostName,
            UserName = _rabbitMQConfig.UserName,
            Password = _rabbitMQConfig.Password,
            Port = int.Parse(_rabbitMQConfig.Port)
        };

        try
        {
            _logger.LogDebug("Attempting to create connection to RabbitMQ");
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();

            _logger.LogDebug("Connection to RabbitMQ established, creating channel");
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

            _logger.LogDebug("Verifying existence of queue '{QueueName}'", _rabbitMQConfig.QueueName);
            _channel.QueueDeclarePassiveAsync(_rabbitMQConfig.QueueName).GetAwaiter().GetResult();

            _logger.LogInformation("RabbitMQ connection established to {HostName}:{Port}", _rabbitMQConfig.HostName, _rabbitMQConfig.Port);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to establish RabbitMQ connection");
            throw;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        try
        {
            _channel.Dispose();
            _connection.Dispose();
            _logger.LogInformation("RabbitMQ connection disposed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing RabbitMQ connection");
        }

        _disposed = true;
    }

    public async Task EnqueueSubmissionAsync(SubmissionMessage submissionMessage, CancellationToken cancellationToken = default)
    {
        submissionMessage.EnsureMessageIsValid();

        var json = JsonConvert.SerializeObject(submissionMessage);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json",
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        };

        await _channel.BasicPublishAsync(string.Empty, _rabbitMQConfig.QueueName, false, properties, body, cancellationToken);

        _logger.LogInformation("Published submission {SubmissionId} to RabbitMQ queue {QueueName}", submissionMessage.SubmissionId, _rabbitMQConfig.QueueName);
    }
}
