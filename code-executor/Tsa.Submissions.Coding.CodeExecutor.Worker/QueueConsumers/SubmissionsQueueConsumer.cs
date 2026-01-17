using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Tsa.Submissions.Coding.CodeExecutor.Shared.Models;
using Tsa.Submissions.Coding.CodeExecutor.Worker.Messages;
using Tsa.Submissions.Coding.CodeExecutor.Worker.Services;

namespace Tsa.Submissions.Coding.CodeExecutor.Worker.QueueConsumers;

internal class SubmissionsQueueConsumer : AsyncDefaultBasicConsumer
{
    private readonly ApiClient _apiClient;
    private readonly CancellationToken _cancellationToken;
    private readonly ILogger _logger;

    public SubmissionsQueueConsumer(ApiClient apiClient, IChannel channel, ILogger logger, CancellationToken cancellationToken) : base(channel)
    {
        _apiClient = apiClient;
        _logger = logger;
        _cancellationToken = cancellationToken;
    }

    public override async Task HandleBasicDeliverAsync(
        string consumerTag,
        ulong deliveryTag,
        bool redelivered,
        string exchange,
        string routingKey,
        IReadOnlyBasicProperties properties,
        ReadOnlyMemory<byte> body,
        CancellationToken cancellationToken = new())
    {
        try
        {
            var messageBody = body.ToArray();
            var message = Encoding.UTF8.GetString(messageBody);
            
            _logger.LogInformation("Received message: {Message}", message);
            _logger.LogDebug("Message details - ConsumerTag: {ConsumerTag}, DeliveryTag: {DeliveryTag}, Exchange: {Exchange}, RoutingKey: {RoutingKey}", consumerTag, deliveryTag, exchange, routingKey);

            // Deserialize the message as SubmissionMessage
            var submissionMessage = System.Text.Json.JsonSerializer.Deserialize<SubmissionMessage>(message);

            if (submissionMessage == null)
            {
                _logger.LogError("Received null or invalid SubmissionMessage. Sending to dead letter queue");
                await Channel.BasicNackAsync(deliveryTag, false, false, cancellationToken);
                return;
            }

            submissionMessage.EnsureMessageIsValid();

            // Fetch and validate submission
            _logger.LogInformation("Fetching submission from API");
            // The null-forgiving operator is safe here due to prior validation
            var submission = await _apiClient.GetSubmissionAsync(submissionMessage.SubmissionId!, cancellationToken);
            submission.EnsureModelIsValid();

            _logger.LogInformation("Processing submission {SubmissionId}", submissionMessage.SubmissionId);
            var success = await ProcessSubmissionAsync(submission, cancellationToken);

            if (success)
            {
                _logger.LogInformation("Successfully processed submission {SubmissionId}", submissionMessage.SubmissionId);
                await Channel.BasicAckAsync(deliveryTag, false, cancellationToken);
            }
            else
            {
                // Business logic failure - send to DLQ for investigation
                _logger.LogError("Processing of submission {SubmissionId} failed. Sending to dead letter queue", submissionMessage.SubmissionId);
                await Channel.BasicNackAsync(deliveryTag, false, false, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            var messageBody = body.ToArray();
            var message = Encoding.UTF8.GetString(messageBody);

            _logger.LogError(ex, "Error processing the message '{Message}'. Sending to dead letter queue", message);
            await Channel.BasicNackAsync(deliveryTag, false, false, cancellationToken);
        }
    }

    private async Task<bool> ProcessSubmissionAsync(SubmissionModel submission, CancellationToken cancellationToken)
    {
        // Placeholder for actual submission processing logic
        await Task.Delay(1000); // Simulate some processing delay
        return true; // Indicate successful processing
    }
}
