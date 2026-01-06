using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MongoDB.Driver;
using Tsa.Submissions.Coding.UnitTests.Data;
using Tsa.Submissions.Coding.UnitTests.Helpers;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Models;
using Tsa.Submissions.Coding.WebApi.Services;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Models;

[ExcludeFromCodeCoverage]
public class ModelExtensions
{
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToEntities_For_ParticipantModels_Should_Return_Participants()
    {
        // Arrange
        var participantModels = new List<ParticipantModel>
        {
            new() { ParticipantNumber = "001", SchoolNumber = "1234" },
            new() { ParticipantNumber = "002", SchoolNumber = "1234" }
        };

        // Act
        var participants = participantModels.ToEntities();

        // Assert
        Assert.Equal(participantModels.Count, participants.Count);

        foreach (var participant in participants)
        {
            Assert.Contains(participantModels, participantModel => participantModel.ParticipantId == participant.ParticipantId);
        }
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToEntities_For_TestSetInputModels_Should_Return_Null_When_Inputs_Is_Null()
    {
        // Arrange
        var testSetModel = new TestSetModel();

        // Act
        var testSetInputs = testSetModel.Inputs.ToEntities();

        // Assert
        Assert.Null(testSetInputs);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToEntities_For_UserModel_Should_Return_User()
    {
        // Arrange
        var usersTestData = new UsersTestData();

        var expectedUser = usersTestData
            .Where(userTestData => (UserDataIssues)userTestData[1] == UserDataIssues.None && ((User)userTestData[0]).Team != null)
            .Select(userTestData => (User)userTestData[0])
            .First();

        var userModel = new UserModel
        {
            Id = expectedUser.Id,
            Role = expectedUser.Role,
            Team = new TeamModel
            {
                CompetitionLevel = expectedUser.Team!.CompetitionLevel.ToString(),
                SchoolNumber = expectedUser.Team!.SchoolNumber,
                TeamNumber = expectedUser.Team!.TeamNumber
            },
            UserName = expectedUser.UserName
        };

        // Act
        var actualUser = userModel.ToEntity();

        // Assert
        Assert.Equal(expectedUser, actualUser, new UserEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToEntity_For_ParticipantModel_Should_Return_Participant()
    {
        // Arrange
        var participantModel = new ParticipantModel
        {
            ParticipantNumber = "123",
            SchoolNumber = "1234"
        };

        // Act
        var participant = participantModel.ToEntity();

        // Assert
        Assert.Equal(participantModel.ParticipantNumber, participant.ParticipantNumber);
        Assert.Equal(participantModel.SchoolNumber, participant.SchoolNumber);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToEntity_For_ProblemModel_Should_Return_Problem()
    {
        // Arrange
        var problemModel = new ProblemModel
        {
            Description = "This is the description",
            Id = "This is the ID",
            IsActive = true,
            Title = "This is the title"
        };

        // Act
        var problem = problemModel.ToEntity();

        // Assert
        Assert.Equal(problemModel.Description, problem.Description);
        Assert.Equal(problemModel.Id, problem.Id);
        Assert.Equal(problemModel.IsActive, problem.IsActive);
        Assert.Equal(problemModel.Title, problem.Title);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToEntity_For_SubmissionModel_Should_Return_Submission()
    {
        // Arrange
        var submissionModel = new SubmissionModel
        {
            Id = "000000000000000000000000",
            IsFinalSubmission = true,
            Language = "csharp",
            ProblemId = "00000000000000000000000A",
            Solution = "The solution",
            SubmittedOn = DateTime.Now.AddHours(-5),
            TestSetResults =
            [
                new TestSetResultModel
                {
                    Passed = true,
                    RunDuration = new TimeSpan(0, 0, 5, 0),
                    TestSetId = "000000000000000000000010"
                },
                new TestSetResultModel
                {
                    Passed = true,
                    RunDuration = new TimeSpan(0, 0, 5, 0),
                    TestSetId = "000000000000000000000011"
                }
            ],
            User = new UserModel
            {
                Id = "000000000000000000000002",
                Role = "participant",
                Team = new TeamModel
                {
                    CompetitionLevel = CompetitionLevel.HighSchool.ToString(),
                    SchoolNumber = "9000",
                    TeamNumber = "901"
                },
                UserName = "9000-901"
            }
        };

        // Act
        var submission = submissionModel.ToEntity();

        Assert.Equal(submissionModel.Id, submission.Id);
        Assert.Equal(submissionModel.IsFinalSubmission, submission.IsFinalSubmission);
        Assert.Equal(submissionModel.Language, submission.Language);
        Assert.Equal(submissionModel.ProblemId, submission.Problem?.Id.AsString);
        Assert.Equal(submissionModel.Solution, submission.Solution);
        Assert.Equal(submissionModel.SubmittedOn, submission.SubmittedOn);
        Assert.NotNull(submission.TestSetResults);
        Assert.Equal(submissionModel.TestSetResults.Count, submission.TestSetResults.Count);

        foreach (var testSetResultModel in submissionModel.TestSetResults)
        {
            var testSetResult = submission.TestSetResults.SingleOrDefault(setResult => setResult.TestSet?.Id.AsString == testSetResultModel.TestSetId);

            Assert.NotNull(testSetResult);
            Assert.Equal(testSetResultModel.Passed, testSetResult.Passed);
            Assert.Equal(testSetResultModel.RunDuration, testSetResult.RunDuration);
            Assert.Equal(testSetResultModel.TestSetId, testSetResult.TestSet?.Id.AsString);
        }
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToEntity_For_TeamModel_Should_Return_Team()
    {
        // Arrange
        var teamModel = new TeamModel
        {
            CompetitionLevel = "MiddleSchool",
            SchoolNumber = "1234",
            TeamNumber = "901"
        };

        var expectedTeam = new Team
        {
            CompetitionLevel = Enum.Parse<CompetitionLevel>(teamModel.CompetitionLevel),
            SchoolNumber = teamModel.SchoolNumber,
            TeamNumber = teamModel.TeamNumber
        };

        // Act
        var actualTeam = teamModel.ToEntity();

        // Assert
        Assert.Equal(expectedTeam, actualTeam, new TeamEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToEntity_For_TestSetInputModel_Should_Return_TestSetInput()
    {
        // Arrange
        var testSetValueModel = new TestSetValueModel
        {
            DataType = "Data Type",
            Index = 9999,
            IsArray = true,
            ValueAsJson = "ValueAsJson"
        };

        var expectedTestSetValue = new TestSetValue
        {
            DataType = testSetValueModel.DataType,
            Index = testSetValueModel.Index,
            IsArray = testSetValueModel.IsArray,
            ValueAsJson = testSetValueModel.ValueAsJson
        };

        // Act
        var actualTestSetInput = testSetValueModel.ToEntity();

        // Assert
        Assert.Equal(expectedTestSetValue, actualTestSetInput, new TestSetValueEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToEntity_For_TestSetModel_Should_Return_TestSet()
    {
        // Arrange
        var testSetModel = new TestSetModel
        {
            Id = "000000000000000000000001",
            Inputs =
            [
                new TestSetValueModel
                {
                    DataType = "Data Type #1",
                    Index = 1,
                    IsArray = true,
                    ValueAsJson = "ValueAsJson #1"
                },
                new TestSetValueModel
                {
                    DataType = "Data Type #2",
                    Index = 2,
                    IsArray = false,
                    ValueAsJson = "ValueAsJson #2"
                },
                new TestSetValueModel
                {
                    DataType = "Data Type #3",
                    Index = 3,
                    IsArray = true,
                    ValueAsJson = "ValueAsJson #3"
                }
            ],
            IsPublic = true,
            Name = "Test Set #1",
            ProblemId = "000000000000000000000001"
        };

        var expectedTestSet = new TestSet
        {
            Id = testSetModel.Id,
            Inputs =
            [
                new TestSetValue
                {
                    DataType = testSetModel.Inputs[0].DataType,
                    Index = testSetModel.Inputs[0].Index,
                    IsArray = testSetModel.Inputs[0].IsArray,
                    ValueAsJson = testSetModel.Inputs[0].ValueAsJson
                },
                new TestSetValue
                {
                    DataType = testSetModel.Inputs[1].DataType,
                    Index = testSetModel.Inputs[1].Index,
                    IsArray = testSetModel.Inputs[1].IsArray,
                    ValueAsJson = testSetModel.Inputs[1].ValueAsJson
                },
                new TestSetValue
                {
                    DataType = testSetModel.Inputs[2].DataType,
                    Index = testSetModel.Inputs[2].Index,
                    IsArray = testSetModel.Inputs[2].IsArray,
                    ValueAsJson = testSetModel.Inputs[2].ValueAsJson
                }
            ],
            IsPublic = testSetModel.IsPublic,
            Name = testSetModel.Name,
            Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, testSetModel.ProblemId)
        };

        // Act
        var actualTestSet = testSetModel.ToEntity();

        // Assert
        Assert.Equal(expectedTestSet, actualTestSet, new TestSetEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToEntity_For_TestSetResultModel_Should_Return_TestSetResult()
    {
        // Arrange
        var testSetResultModel = new TestSetResultModel
        {
            Passed = true,
            RunDuration = new TimeSpan(0, 0, 5, 0),
            TestSetId = "000000000000000000000010"
        };

        var expectedTestSetResult = new TestSetResult
        {
            Passed = testSetResultModel.Passed,
            RunDuration = testSetResultModel.RunDuration,
            TestSet = new MongoDBRef(TestSetsService.MongoDbCollectionName, testSetResultModel.TestSetId)
        };

        // Act
        var actualTestSetResult = testSetResultModel.ToEntity();

        // Assert
        Assert.Equal(expectedTestSetResult, actualTestSetResult, new TestSetResultEqualityComparer());
    }
}
