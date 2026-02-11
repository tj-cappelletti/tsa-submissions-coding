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
public class SubmissionsServiceTest
{
    private const string CollectionName = "submissions";
    private const string DatabaseName = "submissions";

    private readonly Mock<ILogger<SubmissionsService>> _mockedLogger = new();

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Collection_Name_Should_Be_Submissions()
    {
        // Arrange
        var (_, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Submission>(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        // Act
        var submissionsService = new SubmissionsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Assert
        Assert.Equal("submissions", submissionsService.CollectionName);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Constructor_Should_Instantiate_Base_Class()
    {
        // Arrange
        var (_, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Submission>(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        // Act
        var submissionsService = new SubmissionsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Assert
        Assert.NotNull(submissionsService);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task CreateAsync_Should_Insert_New_Entity()
    {
        // Arrange
        var submissionsTestData = new SubmissionsTestData();

        var submission =
            submissionsTestData.First(submissionTestData => (SubmissionDataIssues)submissionTestData[1] == SubmissionDataIssues.None)[0] as Submission;

        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Submission>(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var submissionsService = new SubmissionsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        await submissionsService.CreateAsync(submission!);

        // Assert
        mockedMongoCollection
            .Verify(mongoCollection =>
                mongoCollection.InsertOneAsync(It.Is(submission, new SubmissionEqualityComparer())!, null, CancellationToken.None), Times.Once);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task ExistsAsync_Should_Return_True()
    {
        // Arrange
        var submissionsTestData = new SubmissionsTestData();

        var expectedSubmission = submissionsTestData
            .Where(submissionTestData => (SubmissionDataIssues)submissionTestData[1] == SubmissionDataIssues.None)
            .Select(submissionTestData => submissionTestData[0])
            .Cast<Submission>()
            .Last();

        var expectedSubmissions = new List<Submission>
        {
            expectedSubmission
        };

        var mockedAsyncCursor = MockHelpers.CreateMockedAsyncCursor(expectedSubmissions);

        var filterDefinitionJson = Builders<Submission>.Filter.Eq(submission => submission.Id, expectedSubmission.Id).RenderToJson();

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Submission>(DatabaseName, CollectionName);

        MockHelpers.SetupMockedMongoCollectionFindAsync(mockedMongoCollection, filterDefinitionJson, mockedAsyncCursor);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var submissionsService = new SubmissionsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await submissionsService.ExistsAsync(expectedSubmission.Id!);

        // Assert
        Assert.True(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task GetAsync_Should_Get_All_Entities()
    {
        // Arrange
        var submissionsTestData = new SubmissionsTestData();

        var expectedSubmissions = submissionsTestData
            .Where(submissionTestData => (SubmissionDataIssues)submissionTestData[1] == SubmissionDataIssues.None)
            .Select(submissionTestData => submissionTestData[0])
            .Cast<Submission>()
            .ToList();

        var mockedAsyncCursor = MockHelpers.CreateMockedAsyncCursor(expectedSubmissions);

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Submission>(DatabaseName, CollectionName);

        MockHelpers.SetupMockedMongoCollectionFindAsync(mockedMongoCollection, mockedAsyncCursor);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var submissionsService = new SubmissionsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await submissionsService.GetAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(expectedSubmissions, result, new SubmissionEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task GetAsync_Should_Get_Entity_By_Id()
    {
        // Arrange
        var submissionsTestData = new SubmissionsTestData();

        var expectedSubmission = submissionsTestData
            .Where(submissionTestData => (SubmissionDataIssues)submissionTestData[1] == SubmissionDataIssues.None)
            .Select(submissionTestData => submissionTestData[0])
            .Cast<Submission>()
            .Last();

        var expectedSubmissions = new List<Submission>
        {
            expectedSubmission
        };

        var mockedAsyncCursor = MockHelpers.CreateMockedAsyncCursor(expectedSubmissions);

        var filterDefinitionJson = Builders<Submission>.Filter.Eq(submission => submission.Id, expectedSubmission.Id).RenderToJson();

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Submission>(DatabaseName, CollectionName);

        MockHelpers.SetupMockedMongoCollectionFindAsync(mockedMongoCollection, filterDefinitionJson, mockedAsyncCursor);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var submissionsService = new SubmissionsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await submissionsService.GetAsync(expectedSubmission.Id!);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedSubmission, result, new SubmissionEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task GetAsync_Should_Get_Entity_By_Ids()
    {
        // Arrange
        var submissionsTestData = new SubmissionsTestData();

        var expectedSubmissions = submissionsTestData
            .Where(submissionTestData => (SubmissionDataIssues)submissionTestData[1] == SubmissionDataIssues.None)
            .Select(submissionTestData => submissionTestData[0])
            .Cast<Submission>()
            .ToList();

        var ids = expectedSubmissions.Select(submission => submission.Id).Cast<string>().ToList();

        var mockedAsyncCursor = MockHelpers.CreateMockedAsyncCursor(expectedSubmissions);

        var filterDefinitionJson = Builders<Submission>.Filter.In(submission => submission.Id, ids).RenderToJson();

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Submission>(DatabaseName, CollectionName);

        MockHelpers.SetupMockedMongoCollectionFindAsync(mockedMongoCollection, filterDefinitionJson, mockedAsyncCursor);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var submissionsService = new SubmissionsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await submissionsService.GetAsync(ids);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedSubmissions, result, new SubmissionEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Ping_Should_Return_False()
    {
        // Arrange
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Submission>(DatabaseName, CollectionName);

        var expectedPingCommand = (Command<BsonDocument>)"{ping:1}";

        MockHelpers.SetupMockedMongoCollectionRunCommandAsyncThrowsException(mockedMongoCollection, expectedPingCommand);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var submissionsService = new SubmissionsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await submissionsService.PingAsync();

        // Assert
        Assert.False(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Ping_Should_Return_True()
    {
        // Arrange
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Submission>(DatabaseName, CollectionName);

        var expectedPingCommand = (Command<BsonDocument>)"{ping:1}";

        MockHelpers.SetupMockedMongoCollectionRunCommandAsync(mockedMongoCollection, expectedPingCommand);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var submissionsService = new SubmissionsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await submissionsService.PingAsync();

        // Assert
        Assert.True(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task RemoveAsync_Should_Delete_Entity_By_Id()
    {
        // Arrange
        var submissionsTestData = new SubmissionsTestData();

        var expectedSubmission = submissionsTestData
            .Where(submissionTestData => (SubmissionDataIssues)submissionTestData[1] == SubmissionDataIssues.None)
            .Select(submissionTestData => submissionTestData[0])
            .Cast<Submission>()
            .Last();

        var expectedFilterDefinitionJson = Builders<Submission>.Filter.Eq(submission => submission.Id, expectedSubmission.Id).RenderToJson();

        Func<FilterDefinition<Submission>, bool> validateFilterDefinitionFunc = filterDefinition =>
        {
            var filterDefinitionJson = filterDefinition.RenderToJson();

            return expectedFilterDefinitionJson == filterDefinitionJson;
        };

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Submission>(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var submissionsService = new SubmissionsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        await submissionsService.RemoveAsync(expectedSubmission);

        // Assert
        mockedMongoCollection
            .Verify(mongoCollection => mongoCollection.DeleteOneAsync(
                    It.Is<FilterDefinition<Submission>>(filterDefinition => validateFilterDefinitionFunc(filterDefinition)),
                    null,
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ServiceName_Name_Should_Be_Submissions()
    {
        // Arrange
        var (_, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Submission>(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        // Act
        var submissionsService = new SubmissionsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Assert
        Assert.Equal("Submissions", submissionsService.ServiceName);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task UpdateAsync_Should_Replace_Entity_By_Id()
    {
        // Arrange
        var submissionsTestData = new SubmissionsTestData();

        var expectedSubmission = submissionsTestData
            .Where(submissionTestData => (SubmissionDataIssues)submissionTestData[1] == SubmissionDataIssues.None)
            .Select(submissionTestData => submissionTestData[0])
            .Cast<Submission>()
            .Last();

        var expectedFilterDefinitionJson = Builders<Submission>.Filter.Eq(submission => submission.Id, expectedSubmission.Id).RenderToJson();

        Func<FilterDefinition<Submission>, bool> validateFilterDefinitionFunc = filterDefinition =>
        {
            var filterDefinitionJson = filterDefinition.RenderToJson();

            return expectedFilterDefinitionJson == filterDefinitionJson;
        };

        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<Submission>(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var submissionsService = new SubmissionsService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        await submissionsService.UpdateAsync(expectedSubmission, default);

        // Assert
        mockedMongoCollection
            .Verify(mongoCollection => mongoCollection.ReplaceOneAsync(
                    It.Is<FilterDefinition<Submission>>(filterDefinition => validateFilterDefinitionFunc(filterDefinition)),
                    expectedSubmission,
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
