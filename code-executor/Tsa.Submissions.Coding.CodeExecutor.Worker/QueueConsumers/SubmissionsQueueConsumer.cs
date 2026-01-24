using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Tsa.Submissions.Coding.ApiClient;
using Tsa.Submissions.Coding.CodeExecutor.Worker.Services;
using Tsa.Submissions.Coding.Contracts.CodeExecutor;
using Tsa.Submissions.Coding.Contracts.Messages;
using Tsa.Submissions.Coding.Contracts.Submissions;
using Tsa.Submissions.Coding.Contracts.TestCases;

namespace Tsa.Submissions.Coding.CodeExecutor.Worker.QueueConsumers;

internal class SubmissionsQueueConsumer : AsyncDefaultBasicConsumer
{
    private readonly ICodingApiClient _codingApiClient;
    private readonly KubernetesJobManager _kubernetesJobManager;
    private readonly ILogger _logger;

    public SubmissionsQueueConsumer(ICodingApiClient codingApiClient, IChannel channel, KubernetesJobManager kubernetesJobManager, ILogger logger) : base(channel)
    {
        _codingApiClient = codingApiClient;
        _kubernetesJobManager = kubernetesJobManager;
        _logger = logger;
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
            _logger.LogDebug("Message details - ConsumerTag: {ConsumerTag}, DeliveryTag: {DeliveryTag}, Exchange: {Exchange}, RoutingKey: {RoutingKey}",
                consumerTag, deliveryTag, exchange, routingKey);

            // Deserialize the message as SubmissionMessage
            var submissionMessage = JsonSerializer.Deserialize<SubmissionMessage>(message);

            if (submissionMessage == null)
            {
                _logger.LogError("Received null or invalid SubmissionMessage. Sending to dead letter queue");
                await Channel.BasicNackAsync(deliveryTag, false, false, cancellationToken);
                return;
            }

            // Fetch and validate submission
            _logger.LogInformation("Fetching submission from API");
            // The null-forgiving operator is safe here due to prior validation
            var submission = await _codingApiClient.Submissions.GetAsync(submissionMessage.SubmissionId, cancellationToken);
            var problem = await _codingApiClient.Problems.GetAsync(submission.ProblemId, true, cancellationToken);

            _logger.LogInformation("Processing submission {SubmissionId}", submissionMessage.SubmissionId);
            var success = await ProcessSubmissionAsync(submission, problem.TestCases, cancellationToken);

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

    private async Task<bool> ProcessSubmissionAsync(SubmissionResponse submission, List<TestCase> testCases, CancellationToken cancellationToken)
    {
        await _kubernetesJobManager.ExecuteJobAsync(
            new RunnerJobPayload(
                submission.Language.Name,
                submission.Language.Version,
                submission.ProblemId,
                submission.Solution,
                submission.Id,
                testCases),
            cancellationToken);

        return true;
    }
}
