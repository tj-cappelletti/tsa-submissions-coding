using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Tsa.Submissions.Coding.WebApi.Configuration;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Services;

public class TestCasesService : MongoDbService<TestCase>, ITestCasesService
{
    public const string MongoDbCollectionName = "problem_test_cases";

    public string CollectionName => MongoDbCollectionName;

    public string ServiceName => "TestCases";

    public TestCasesService(IMongoClient mongoClient, IOptions<SubmissionsDatabase> options, ILogger<TestCasesService> logger) : base(
        mongoClient,
        options.Value.Name!,
        MongoDbCollectionName,
        logger) { }

    public string ComputeSignature(TestCase testCase)
    {
        var orderedTestCaseInputs = testCase.Inputs
            .OrderBy(testCaseInput => testCaseInput.Index)
            .Select(i => new Dictionary<string, object?>
            {
                ["index"] = i.Index,
                ["dataType"] = i.DataType,
                ["isArray"] = i.IsArray,
                ["value"] = i.Value
            })
            .ToList();

        var hashPayload = new Dictionary<string, object?>
        {
            ["inputs"] = orderedTestCaseInputs,
            ["expectedOutput"] = testCase.ExpectedOutput,
            ["outputDataType"] = testCase.OutputDataType,
            ["outputIsArray"] = testCase.OutputIsArray
        };

        var jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        var serializedHashPayload = JsonSerializer.Serialize(hashPayload, jsonSerializerOptions);

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(serializedHashPayload));
        return "sha256:" + Convert.ToHexString(hash).ToLowerInvariant();
    }

    public async Task<List<TestCase>> GetByProblemAsync(Problem problem, CancellationToken cancellationToken = default)
    {
        var filterDefinition = Builders<TestCase>.Filter.Eq(testCase => testCase.ProblemId, problem.Id);

        var cursor = await EntityCollection.FindAsync(filterDefinition, cancellationToken: cancellationToken);

        var results = await cursor.ToListAsync(cancellationToken);

        return results;
    }
}
