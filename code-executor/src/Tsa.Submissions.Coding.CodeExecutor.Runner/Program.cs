using System.Text.Json;
using Tsa.Submissions.Coding.CodeExecutor.Runner.Services;
using Tsa.Submissions.Coding.CodeExecutor.Shared.Models;

try
{
    // Read execution payload from environment variable
    var payloadJson = Environment.GetEnvironmentVariable("EXECUTION_PAYLOAD");
    
    if (string.IsNullOrEmpty(payloadJson))
    {
        Console.Error.WriteLine("ERROR: EXECUTION_PAYLOAD environment variable not set");
        return 1;
    }

    // Deserialize payload
    var payload = JsonSerializer.Deserialize<ExecutionPayload>(payloadJson);
    
    if (payload == null)
    {
        Console.Error.WriteLine("ERROR: Failed to deserialize execution payload");
        return 1;
    }

    // Run test cases
    var runner = new TestCaseRunner();
    var result = await runner.RunTestCasesAsync(payload);

    // Output results as JSON to stdout
    var resultJson = JsonSerializer.Serialize(result, new JsonSerializerOptions
    {
        WriteIndented = false
    });
    
    Console.WriteLine(resultJson);
    
    return result.Success ? 0 : 1;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"FATAL ERROR: {ex.Message}");
    Console.Error.WriteLine(ex.StackTrace);
    return 1;
}
