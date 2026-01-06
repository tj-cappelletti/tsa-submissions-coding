using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Tsa.Submissions.Coding.UnitTests.Data;
using Tsa.Submissions.Coding.UnitTests.ExtensionMethods;
using Tsa.Submissions.Coding.UnitTests.Helpers;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Services;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Services;

[ExcludeFromCodeCoverage]
public class ProblemsServiceTest
{
    private const string CollectionName = "problems";
    private const string DatabaseName = "submissions";

    private readonly Mock<ILogger<ProblemsService>> _mockedLogger = new();

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Collection_Name_Should_Be_Problems()
    {
        // Arrange
        var (_, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Problem>(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        // Act
        var problemsService = new ProblemsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Assert
        Assert.Equal("problems", problemsService.CollectionName);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Constructor_Should_Instantiate_Base_Class()
    {
        // Arrange
        var (_, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Problem>(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        // Act
        var problemsService = new ProblemsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Assert
        Assert.NotNull(problemsService);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task CreateAsync_Should_Insert_New_Entity()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problem = problemsTestData.First(problemTestData => (ProblemDataIssues)problemTestData[1] == ProblemDataIssues.None)[0] as Problem;

        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Problem>(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var problemsService = new ProblemsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        await problemsService.CreateAsync(problem!);

        // Assert
        mockedMongoCollection
            .Verify(mongoCollection =>
                mongoCollection.InsertOneAsync(It.Is(problem, new ProblemEqualityComparer())!, null, CancellationToken.None), Times.Once);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task ExistsAsync_Should_Return_True()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var expectedProblem = problemsTestData
            .Where(problemTestData => (ProblemDataIssues)problemTestData[1] == ProblemDataIssues.None)
            .Select(problemTestData => problemTestData[0])
            .Cast<Problem>()
            .Last();

        var problems = new List<Problem>
        {
            expectedProblem
        };

        var mockedAsyncCursor = MockHelpers.CreateMockedAsyncCursor(problems);

        var filterDefinitionJson = Builders<Problem>.Filter.Eq(problem => problem.Id, expectedProblem.Id).RenderToJson();

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Problem>(DatabaseName, CollectionName);

        MockHelpers.SetupMockedMongoCollectionFindAsync(mockedMongoCollection, filterDefinitionJson, mockedAsyncCursor);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var problemsService = new ProblemsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await problemsService.ExistsAsync(expectedProblem.Id!);

        // Assert
        Assert.True(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task GetAsync_Should_Get_All_Entities()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problems = problemsTestData
            .Where(problemTestData => (ProblemDataIssues)problemTestData[1] == ProblemDataIssues.None)
            .Select(problemTestData => problemTestData[0])
            .Cast<Problem>()
            .ToList();

        var mockedAsyncCursor = MockHelpers.CreateMockedAsyncCursor(problems);

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Problem>(DatabaseName, CollectionName);

        MockHelpers.SetupMockedMongoCollectionFindAsync(mockedMongoCollection, mockedAsyncCursor);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var problemsService = new ProblemsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await problemsService.GetAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(problems.Count, result.Count);
        Assert.Equal(problems, result, new ProblemEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task GetAsync_Should_Get_Entity_By_Id()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var expectedProblem = problemsTestData
            .Where(problemTestData => (ProblemDataIssues)problemTestData[1] == ProblemDataIssues.None)
            .Select(problemTestData => problemTestData[0])
            .Cast<Problem>()
            .Last();

        var problems = new List<Problem>
        {
            expectedProblem
        };

        var mockedAsyncCursor = MockHelpers.CreateMockedAsyncCursor(problems);

        var filterDefinitionJson = Builders<Problem>.Filter.Eq(problem => problem.Id, expectedProblem.Id).RenderToJson();

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Problem>(DatabaseName, CollectionName);

        MockHelpers.SetupMockedMongoCollectionFindAsync(mockedMongoCollection, filterDefinitionJson, mockedAsyncCursor);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var problemsService = new ProblemsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await problemsService.GetAsync(expectedProblem.Id!);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedProblem, result, new ProblemEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task GetAsync_Should_Get_Entity_By_Ids()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var expectedProblems = problemsTestData
            .Where(problemTestData => (ProblemDataIssues)problemTestData[1] == ProblemDataIssues.None)
            .Select(problemTestData => problemTestData[0])
            .Cast<Problem>()
            .ToList();

        var ids = expectedProblems.Select(problem => problem.Id).Cast<string>().ToList();

        var mockedAsyncCursor = MockHelpers.CreateMockedAsyncCursor(expectedProblems);

        var filterDefinitionJson = Builders<Problem>.Filter.In(problem => problem.Id, ids).RenderToJson();

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Problem>(DatabaseName, CollectionName);

        MockHelpers.SetupMockedMongoCollectionFindAsync(mockedMongoCollection, filterDefinitionJson, mockedAsyncCursor);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var problemsService = new ProblemsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await problemsService.GetAsync(ids);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedProblems.Count, result.Count);
        Assert.Equal(expectedProblems, result, new ProblemEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Ping_Should_Return_False()
    {
        // Arrange
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Problem>(DatabaseName, CollectionName);

        var expectedPingCommand = (Command<BsonDocument>)"{ping:1}";

        MockHelpers.SetupMockedMongoCollectionRunCommandAsyncThrowsException(mockedMongoCollection, expectedPingCommand);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var problemsService = new ProblemsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await problemsService.PingAsync();

        // Assert
        Assert.False(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Ping_Should_Return_True()
    {
        // Arrange
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Problem>(DatabaseName, CollectionName);

        var expectedPingCommand = (Command<BsonDocument>)"{ping:1}";

        MockHelpers.SetupMockedMongoCollectionRunCommandAsync(mockedMongoCollection, expectedPingCommand);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var problemsService = new ProblemsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await problemsService.PingAsync();

        // Assert
        Assert.True(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task RemoveAsync_Should_Delete_Entity_By_Id()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var expectedProblem = problemsTestData
            .Where(problemTestData => (ProblemDataIssues)problemTestData[1] == ProblemDataIssues.None)
            .Select(problemTestData => problemTestData[0])
            .Cast<Problem>()
            .Last();

        var expectedFilterDefinitionJson = Builders<Problem>.Filter.Eq(problem => problem.Id, expectedProblem.Id).RenderToJson();

        Func<FilterDefinition<Problem>, bool> validateFilterDefinitionFunc = filterDefinition =>
        {
            var filterDefinitionJson = filterDefinition.RenderToJson();

            return expectedFilterDefinitionJson == filterDefinitionJson;
        };

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Problem>(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var problemsService = new ProblemsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        await problemsService.RemoveAsync(expectedProblem);

        // Assert
        mockedMongoCollection
            .Verify(mongoCollection => mongoCollection.DeleteOneAsync(
                    It.Is<FilterDefinition<Problem>>(fd => validateFilterDefinitionFunc(fd)),
                    null,
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ServiceName_Name_Should_Be_Problems()
    {
        // Arrange
        var (_, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Problem>(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        // Act
        var problemsService = new ProblemsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Assert
        Assert.Equal("Problems", problemsService.ServiceName);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task UpdateAsync_Should_Replace_Entity_By_Id()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var expectedProblem = problemsTestData
            .Where(problemTestData => (ProblemDataIssues)problemTestData[1] == ProblemDataIssues.None)
            .Select(problemTestData => problemTestData[0])
            .Cast<Problem>()
            .Last();

        var expectedFilterDefinitionJson = Builders<Problem>.Filter.Eq(problem => problem.Id, expectedProblem.Id).RenderToJson();

        Func<FilterDefinition<Problem>, bool> validateFilterDefinitionFunc = filterDefinition =>
        {
            var filterDefinitionJson = filterDefinition.RenderToJson();

            return expectedFilterDefinitionJson == filterDefinitionJson;
        };

        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Problem>(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var problemsService = new ProblemsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        await problemsService.UpdateAsync(expectedProblem, default);

        // Assert
        mockedMongoCollection
            .Verify(mongoCollection => mongoCollection.ReplaceOneAsync(
                    It.Is<FilterDefinition<Problem>>(fd => validateFilterDefinitionFunc(fd)),
                    expectedProblem,
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
