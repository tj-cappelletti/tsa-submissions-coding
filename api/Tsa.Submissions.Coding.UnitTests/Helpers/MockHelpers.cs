using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Tsa.Submissions.Coding.UnitTests.ExtensionMethods;
using Tsa.Submissions.Coding.WebApi.Configuration;

namespace Tsa.Submissions.Coding.UnitTests.Helpers;

[ExcludeFromCodeCoverage]
internal static class MockHelpers
{
    public static Mock<IAsyncCursor<T>> CreateMockedAsyncCursor<T>(List<T> documents)
    {
        var mockedAsyncCursor = new Mock<IAsyncCursor<T>>();

        mockedAsyncCursor
            .Setup(cursor => cursor.Current)
            .Returns(documents);

        mockedAsyncCursor
            .SetupSequence(cursor => cursor.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);

        mockedAsyncCursor
            .SetupSequence(asyncCursor => asyncCursor.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        return mockedAsyncCursor;
    }

    public static (Mock<IMongoCollection<T>>, Mock<IMongoClient>) CreateMockedMongoObjects<T>(string databaseName, string collectionName)
    {
        var mockedMongoDatabase = new Mock<IMongoDatabase>();

        var mockedMongoCollection = new Mock<IMongoCollection<T>>();
        mockedMongoCollection
            .Setup(mongoCollection => mongoCollection.Database)
            .Returns(mockedMongoDatabase.Object);

        mockedMongoDatabase
            .Setup(mongoDatabase => mongoDatabase.GetCollection<T>(It.Is<string>(name => name == collectionName), null))
            .Returns(mockedMongoCollection.Object);

        var mockedMongoClient = new Mock<IMongoClient>();
        mockedMongoClient
            .Setup(mongoClient => mongoClient.GetDatabase(It.Is<string>(name => name == databaseName), null))
            .Returns(mockedMongoDatabase.Object);

        return (mockedMongoCollection, mockedMongoClient);
    }

    public static Mock<IOptions<SubmissionsDatabase>> CreateMockedSubmissionsDatabaseOptions(string databaseName)
    {
        var mockedSubmissionsDatabaseOptions = new Mock<IOptions<SubmissionsDatabase>>();
        mockedSubmissionsDatabaseOptions
            .Setup(options => options.Value)
            .Returns(new SubmissionsDatabase
            {
                Name = databaseName
            });

        return mockedSubmissionsDatabaseOptions;
    }

    public static void SetupMockedMongoCollectionFindAsync<T>(Mock<IMongoCollection<T>> mockedMongoCollection, string filterDefinitionJson,
        Mock<IAsyncCursor<T>> mockedAsyncCursor)
    {
        mockedMongoCollection
            .Setup(mongoCollection => mongoCollection.FindAsync(
                It.Is<FilterDefinition<T>>(filter => filter.RenderToJson().Equals(filterDefinitionJson)),
                It.IsAny<FindOptions<T, T>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockedAsyncCursor.Object)
            .Verifiable();
    }

    public static void SetupMockedMongoCollectionFindAsync<T>(Mock<IMongoCollection<T>> mockedMongoCollection, Mock<IAsyncCursor<T>> mockedAsyncCursor)
    {
        mockedMongoCollection
            .Setup(mongoCollection => mongoCollection.FindAsync(
                Builders<T>.Filter.Empty,
                It.IsAny<FindOptions<T, T>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockedAsyncCursor.Object)
            .Verifiable();
    }

    public static void SetupMockedMongoCollectionRunCommandAsync<T>(Mock<IMongoCollection<T>> mockedMongoCollection,
        Command<BsonDocument> expectedPingCommand)
    {
        mockedMongoCollection
            .Setup(mongoCollection =>
                mongoCollection.Database.RunCommandAsync(It.Is<Command<BsonDocument>>(command => ValidatePingCommand(expectedPingCommand, command)),
                    It.IsAny<ReadPreference>(), It.IsAny<CancellationToken>()))
            .Verifiable(Times.Once);
    }

    public static void SetupMockedMongoCollectionRunCommandAsyncThrowsException<T>(Mock<IMongoCollection<T>> mockedMongoCollection,
        Command<BsonDocument> expectedPingCommand)
    {
        mockedMongoCollection
            .Setup(mongoCollection =>
                mongoCollection.Database.RunCommandAsync(It.Is<Command<BsonDocument>>(command => ValidatePingCommand(expectedPingCommand, command)),
                    It.IsAny<ReadPreference>(), It.IsAny<CancellationToken>()))
            .Throws<Exception>()
            .Verifiable(Times.Once);
    }

    public static bool ValidatePingCommand(Command<BsonDocument> expectedCommand, Command<BsonDocument> actualCommand)
    {
        var expectedCommandJson = expectedCommand.ToJson();
        var actualCommandJson = actualCommand.ToJson();

        return expectedCommandJson == actualCommandJson;
    }
}
