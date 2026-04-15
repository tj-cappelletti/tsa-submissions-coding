using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Tsa.Submissions.Coding.ApiClient;
using Tsa.Submissions.Coding.CodeExecutor.Worker.Configuration;
using Tsa.Submissions.Coding.CodeExecutor.Worker.Orchestrator;
using Tsa.Submissions.Coding.CodeExecutor.Worker.Services;
using Tsa.Submissions.Coding.CodeExecutor.Worker.Strategies;
using Tsa.Submissions.Coding.Contracts.CodeExecutor;
using Tsa.Submissions.Coding.Contracts.Messages;
using Tsa.Submissions.Coding.Contracts.ProblemLanguageVariants;
using Tsa.Submissions.Coding.Contracts.Submissions;
using Tsa.Submissions.Coding.Contracts.TestCases;

namespace Tsa.Submissions.Coding.CodeExecutor.Worker.QueueConsumers;

internal class SubmissionsQueueConsumer : AsyncDefaultBasicConsumer
{
    private readonly ICodingApiClient _codingApiClient;
    private readonly KubernetesOrchestrator _kubernetesOrchestrator;
    private readonly ILogger _logger;
    private readonly RunnerImageRegistry _runnerImageRegistry;
    private readonly ScorerImageRegistry _scorerImageRegistry;

    public SubmissionsQueueConsumer(
        ICodingApiClient codingApiClient,
        IChannel channel,
        KubernetesOrchestrator kubernetesOrchestrator,
        ILogger logger,
        IOptions<RunnerImageRegistry> runnerImageRegistry,
        IOptions<ScorerImageRegistry> scorerImageRegistry) :
        base(channel)
    {
        _codingApiClient = codingApiClient;
        _kubernetesOrchestrator = kubernetesOrchestrator;
        _logger = logger;

        //TODO: Refactor to inject a list of registries or a registry provider instead of individual registries
        _runnerImageRegistry = runnerImageRegistry.Value;
        _scorerImageRegistry = scorerImageRegistry.Value;
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

            // Fetch problem and submission to create the job payload
            _logger.LogInformation("Fetching submission from API");
            var submission = await _codingApiClient.Submissions.GetAsync(submissionMessage.SubmissionId, cancellationToken);

            if (submission.EvaluatedOn != null)
            {
                _logger.LogInformation("Submission {SubmissionId} has already been evaluated. Skipping processing.", submissionMessage.SubmissionId);
                
                await Channel.BasicAckAsync(deliveryTag, false, cancellationToken);
                return;
            }

            _logger.LogInformation("Fetching problem and language variant from API");
            
            var problem = await _codingApiClient.Problems.GetAsync(submission.Problem.Id, true, cancellationToken);
            var problemLanguageVariant = await _codingApiClient.Problems.GetLanguageVariantAsync(
                problem.Id,
                submission.ProgrammingLanguage.Id,
                submission.ProgrammingLanguageVersionTag,
                cancellationToken);

            var runnerJobPayload = new RunnerJobPayload(
                submission.ProgrammingLanguage.Name,
                problemLanguageVariant.TestHarnessCode,
                submission.ProgrammingLanguageVersionTag,
                submission.Problem.Id,
                submission.Solution,
                submission.Id,
                problem.TestCases ?? [],
                problemLanguageVariant.WorkspaceFiles);

            var runnerJobStrategy = new RunnerJobStrategy(_runnerImageRegistry);

            var runnerResult = await _kubernetesOrchestrator.ExecuteJobAsync(
                runnerJobStrategy,
                runnerJobPayload,
                cancellationToken);

            // This call is now safe to retry if it fails
            await _codingApiClient.Submissions.PostTestCaseResultsAsync(
                submission.Id,
                runnerResult.TestCaseResults.ToList(),
                cancellationToken);

            if (runnerResult.IsSuccess)
            {
                var scorerJobStrategy = new ScorerJobStrategy(_scorerImageRegistry);

                var scorerResult = await _kubernetesOrchestrator.ExecuteJobAsync(
                    scorerJobStrategy,
                    runnerJobPayload,
                    cancellationToken);
            }

            await Channel.BasicAckAsync(deliveryTag, false, cancellationToken);
        }
        catch (Exception ex)
        {
            var messageBody = body.ToArray();
            var message = Encoding.UTF8.GetString(messageBody);

            _logger.LogError(ex, "Error processing the message '{Message}'. Sending to dead letter queue", message);
            await Channel.BasicNackAsync(deliveryTag, false, false, cancellationToken);
        }
    }

    //private async Task<bool> ProcessSubmissionAsync(
    //    SubmissionResponse submission,
    //    ProblemLanguageVariantResponse problemLanguageVariant,
    //    List<TestCaseResponse> testCases,
    //    CancellationToken cancellationToken)
    //{
    //    var runnerJobPayload = new RunnerJobPayload(
    //        submission.ProgrammingLanguage.Name,
    //        problemLanguageVariant.TestHarnessCode,
    //        submission.ProgrammingLanguageVersionTag,
    //        submission.Problem.Id,
    //        submission.Solution,
    //        submission.Id,
    //        testCases,
    //        problemLanguageVariant.WorkspaceFiles);

    //    await _kubernetesJobManager.ExecuteJobAsync(runnerJobPayload, cancellationToken);

    //    return true;
    //}
}
