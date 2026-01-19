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
public class UsersServiceTest
{
    private const string CollectionName = "users";
    private const string DatabaseName = "submissions";

    private readonly Mock<ILogger<UsersService>> _mockedLogger = new();

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Collection_Name_Should_Be_Users()
    {
        // Arrange
        var (_, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<User>(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        // Act
        var usersService = new UsersService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Assert
        Assert.Equal("users", usersService.CollectionName);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Constructor_Should_Instantiate_Base_Class()
    {
        // Arrange
        var (_, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<User>(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        // Act
        var usersService = new UsersService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Assert
        Assert.NotNull(usersService);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task CreateAsync_Should_Insert_New_Entity()
    {
        // Arrange
        var usersTestData = new UsersTestData();

        var user = usersTestData.First(userTestData => (UserDataIssues)userTestData[1] == UserDataIssues.None)[0] as User;

        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<User>(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var usersService = new UsersService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        await usersService.CreateAsync(user!);

        // Assert
        mockedMongoCollection
            .Verify(mongoCollection =>
                mongoCollection.InsertOneAsync(It.Is(user, new UserEqualityComparer())!, null, CancellationToken.None), Times.Once);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task ExistsAsync_Should_Return_True()
    {
        // Arrange
        var usersTestData = new UsersTestData();

        var expectedUser = usersTestData
            .Where(userTestData => (UserDataIssues)userTestData[1] == UserDataIssues.None)
            .Select(userTestData => userTestData[0])
            .Cast<User>()
            .Last();

        var users = new List<User>
        {
            expectedUser
        };

        var mockedAsyncCursor = MockHelpers.CreateMockedAsyncCursor(users);

        var filterDefinitionJson = Builders<User>.Filter.Eq(user => user.Id, expectedUser.Id).RenderToJson();

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<User>(DatabaseName, CollectionName);

        MockHelpers.SetupMockedMongoCollectionFindAsync(mockedMongoCollection, filterDefinitionJson, mockedAsyncCursor);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var usersService = new UsersService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await usersService.ExistsAsync(expectedUser.Id!);

        // Assert
        Assert.True(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task GetAsync_Should_Get_All_Entities()
    {
        // Arrange
        var usersTestData = new UsersTestData();

        var users = usersTestData
            .Where(userTestData => (UserDataIssues)userTestData[1] == UserDataIssues.None)
            .Select(userTestData => userTestData[0])
            .Cast<User>()
            .ToList();

        var mockedAsyncCursor = MockHelpers.CreateMockedAsyncCursor(users);

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<User>(DatabaseName, CollectionName);

        MockHelpers.SetupMockedMongoCollectionFindAsync(mockedMongoCollection, mockedAsyncCursor);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var usersService = new UsersService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await usersService.GetAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(users.Count, result.Count);
        Assert.Equal(users, result, new UserEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task GetAsync_Should_Get_Entity_By_Id()
    {
        // Arrange
        var usersTestData = new UsersTestData();

        var expectedUser = usersTestData
            .Where(userTestData => (UserDataIssues)userTestData[1] == UserDataIssues.None)
            .Select(userTestData => userTestData[0])
            .Cast<User>()
            .Last();

        var users = new List<User>
        {
            expectedUser
        };

        var mockedAsyncCursor = MockHelpers.CreateMockedAsyncCursor(users);

        var filterDefinitionJson = Builders<User>.Filter.Eq(user => user.Id, expectedUser.Id).RenderToJson();

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<User>(DatabaseName, CollectionName);

        MockHelpers.SetupMockedMongoCollectionFindAsync(mockedMongoCollection, filterDefinitionJson, mockedAsyncCursor);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var usersService = new UsersService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await usersService.GetAsync(expectedUser.Id!);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUser, result, new UserEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task GetByUserNameAsync_Should_Get_Entity_By_UserName()
    {
        // Arrange
        var usersTestData = new UsersTestData();

        var expectedUser = usersTestData
            .Where(userTestData => (UserDataIssues)userTestData[1] == UserDataIssues.None)
            .Select(userTestData => userTestData[0])
            .Cast<User>()
            .Last();

        var users = new List<User>
        {
            expectedUser
        };

        var mockedAsyncCursor = MockHelpers.CreateMockedAsyncCursor(users);

        var filterDefinitionJson = Builders<User>.Filter.Eq(user => user.UserName, expectedUser.UserName).RenderToJson();

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<User>(DatabaseName, CollectionName);

        MockHelpers.SetupMockedMongoCollectionFindAsync(mockedMongoCollection, filterDefinitionJson, mockedAsyncCursor);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var usersService = new UsersService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await usersService.GetByUserNameAsync(expectedUser.UserName!);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUser, result, new UserEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task GetAsync_Should_Get_Entity_By_Ids()
    {
        // Arrange
        var usersTestData = new UsersTestData();

        var expectedUsers = usersTestData
            .Where(userTestData => (UserDataIssues)userTestData[1] == UserDataIssues.None)
            .Select(userTestData => userTestData[0])
            .Cast<User>()
            .ToList();

        var ids = expectedUsers.Select(user => user.Id).Cast<string>().ToList();

        var mockedAsyncCursor = MockHelpers.CreateMockedAsyncCursor(expectedUsers);

        var filterDefinitionJson = Builders<User>.Filter.In(user => user.Id, ids).RenderToJson();

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<User>(DatabaseName, CollectionName);

        MockHelpers.SetupMockedMongoCollectionFindAsync(mockedMongoCollection, filterDefinitionJson, mockedAsyncCursor);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var usersService = new UsersService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await usersService.GetAsync(ids);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUsers.Count, result.Count);
        Assert.Equal(expectedUsers, result, new UserEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Ping_Should_Return_False()
    {
        // Arrange
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<User>(DatabaseName, CollectionName);

        var expectedPingCommand = (Command<BsonDocument>)"{ping:1}";

        MockHelpers.SetupMockedMongoCollectionRunCommandAsyncThrowsException(mockedMongoCollection, expectedPingCommand);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var usersService = new UsersService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await usersService.PingAsync();

        // Assert
        Assert.False(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Ping_Should_Return_True()
    {
        // Arrange
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<User>(DatabaseName, CollectionName);

        var expectedPingCommand = (Command<BsonDocument>)"{ping:1}";

        MockHelpers.SetupMockedMongoCollectionRunCommandAsync(mockedMongoCollection, expectedPingCommand);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var usersService = new UsersService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        var result = await usersService.PingAsync();

        // Assert
        Assert.True(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task RemoveAsync_Should_Delete_Entity_By_Id()
    {
        // Arrange
        var usersTestData = new UsersTestData();

        var expectedUser = usersTestData
            .Where(userTestData => (UserDataIssues)userTestData[1] == UserDataIssues.None)
            .Select(userTestData => userTestData[0])
            .Cast<User>()
            .Last();

        var expectedFilterDefinitionJson = Builders<User>.Filter.Eq(user => user.Id, expectedUser.Id).RenderToJson();

        Func<FilterDefinition<User>, bool> validateFilterDefinitionFunc = filterDefinition =>
        {
            var filterDefinitionJson = filterDefinition.RenderToJson();

            return expectedFilterDefinitionJson == filterDefinitionJson;
        };

        // If you get this error:
        // System.ArgumentNullException : Value cannot be null. (Parameter 'source')
        // The predicate for FindAsync changed and is causing an error
        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<User>(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var usersService = new UsersService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        await usersService.RemoveAsync(expectedUser);

        // Assert
        mockedMongoCollection
            .Verify(mongoCollection => mongoCollection.DeleteOneAsync(
                    It.Is<FilterDefinition<User>>(fd => validateFilterDefinitionFunc(fd)),
                    null,
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ServiceName_Name_Should_Be_Users()
    {
        // Arrange
        var (_, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<User>(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        // Act
        var usersService = new UsersService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Assert
        Assert.Equal("Users", usersService.ServiceName);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task UpdateAsync_Should_Replace_Entity_By_Id()
    {
        // Arrange
        var usersTestData = new UsersTestData();

        var expectedUser = usersTestData
            .Where(userTestData => (UserDataIssues)userTestData[1] == UserDataIssues.None)
            .Select(userTestData => userTestData[0])
            .Cast<User>()
            .Last();

        var expectedFilterDefinitionJson = Builders<User>.Filter.Eq(user => user.Id, expectedUser.Id).RenderToJson();

        Func<FilterDefinition<User>, bool> validateFilterDefinitionFunc = filterDefinition =>
        {
            var filterDefinitionJson = filterDefinition.RenderToJson();

            return expectedFilterDefinitionJson == filterDefinitionJson;
        };

        var (mockedMongoCollection, mockedMongoClient) = MockHelpers.CreateMockedMongoObjects<User>(DatabaseName, CollectionName);

        var mockedSubmissionsDatabaseOptions = MockHelpers.CreateMockedSubmissionsDatabaseOptions(DatabaseName);

        var usersService = new UsersService(mockedMongoClient.Object, mockedSubmissionsDatabaseOptions.Object, _mockedLogger.Object);

        // Act
        await usersService.UpdateAsync(expectedUser, default);

        // Assert
        mockedMongoCollection
            .Verify(mongoCollection => mongoCollection.ReplaceOneAsync(
                    It.Is<FilterDefinition<User>>(fd => validateFilterDefinitionFunc(fd)),
                    expectedUser,
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
