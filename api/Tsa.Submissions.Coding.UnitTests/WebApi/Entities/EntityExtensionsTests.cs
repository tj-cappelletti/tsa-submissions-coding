using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MongoDB.Driver;
using Tsa.Submissions.Coding.UnitTests.Data;
using Tsa.Submissions.Coding.UnitTests.Helpers;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Models;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Entities;

[ExcludeFromCodeCoverage]
public class EntityExtensions
{
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModel_For_Participant_Should_Return_ParticipantModel()
    {
        // Arrange
        var participant = new Participant
        {
            ParticipantNumber = "123",
            SchoolNumber = "1234"
        };

        var expectedParticipantModel = new ParticipantModel
        {
            ParticipantNumber = participant.ParticipantNumber,
            SchoolNumber = participant.SchoolNumber
        };

        // Act
        var actualParticipantModel = participant.ToModel();

        // Assert
        Assert.Equal(expectedParticipantModel, actualParticipantModel, new ParticipantModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModel_For_Problem_Should_Return_ProblemModel()
    {
        // Arrange
        var problemsTestData = new ProblemsTestData();

        var problem = problemsTestData
            .Where(problemTestData => (ProblemDataIssues)problemTestData[1] == ProblemDataIssues.None)
            .Select(problemTestData => problemTestData[0])
            .Cast<Problem>()
            .Last();

        var expectedProblemModel = new ProblemModel
        {
            Description = problem.Description,
            Id = problem.Id,
            IsActive = problem.IsActive,
            Title = problem.Title
        };

        // Act
        var actualProblemModel = problem.ToModel();

        // Assert
        Assert.Equal(expectedProblemModel, actualProblemModel, new ProblemModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModel_For_Submission_Should_Return_SubmissionModel()
    {
        // Arrange
        var submissionsTestData = new SubmissionsTestData();

        var submission = submissionsTestData
            .Where(submissionTestData => (SubmissionDataIssues)submissionTestData[1] == SubmissionDataIssues.None)
            .Select(submissionTestData => submissionTestData[0])
            .Cast<Submission>()
            .Last();

        var expectedSubmissionModel = new SubmissionModel
        {
            Id = submission.Id,
            IsFinalSubmission = submission.IsFinalSubmission,
            Language = submission.Language,
            ProblemId = submission.Problem?.Id.AsString,
            Solution = submission.Solution,
            SubmittedOn = submission.SubmittedOn,
            TestSetResults = new List<TestSetResultModel>(),
            User = new UserModel { Id = submission.User?.Id.AsString }
        };

        submission.TestSetResults =
        [
            new TestSetResult
            {
                Passed = true,
                RunDuration = new TimeSpan(0, 0, 5, 0),
                TestSet = new MongoDBRef("test-sets", "000000000000000000000010")
            },
            new TestSetResult
            {
                Passed = true,
                RunDuration = new TimeSpan(0, 0, 5, 0),
                TestSet = new MongoDBRef("test-sets", "000000000000000000000011")
            }
        ];

        foreach (var submissionTestSetResult in submission.TestSetResults)
        {
            expectedSubmissionModel.TestSetResults.Add(new TestSetResultModel
            {
                Passed = submissionTestSetResult.Passed,
                RunDuration = submissionTestSetResult.RunDuration,
                TestSetId = submissionTestSetResult.TestSet?.Id.AsString
            });
        }

        // Act
        var actualSubmissionModel = submission.ToModel();

        // Assert
        Assert.Equal(expectedSubmissionModel, actualSubmissionModel, new SubmissionModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModel_For_Team_Should_Return_TeamModel()
    {
        // Arrange
        var teamsTestData = new TeamsTestData();

        var team = teamsTestData
            .Where(teamTestData => (TeamDataIssues)teamTestData[1] == TeamDataIssues.None)
            .Select(teamTestData => teamTestData[0])
            .Cast<Team>()
            .Last();

        var expectedTeamModel = new TeamModel
        {
            CompetitionLevel = team.CompetitionLevel.ToString(),
            SchoolNumber = team.SchoolNumber,
            TeamNumber = team.TeamNumber
        };

        // Act
        var actualTeamModel = team.ToModel();

        // Assert
        Assert.Equal(expectedTeamModel, actualTeamModel, new TeamModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModel_For_TestSet_Should_Return_TestSetModel()
    {
        // Arrange
        var testSetsTestData = new TestSetsTestData();

        var testSet = testSetsTestData
            .Where(testSetTestData => (TestSetDataIssues)testSetTestData[1] == TestSetDataIssues.None)
            .Select(testSetTestData => testSetTestData[0])
            .Cast<TestSet>()
            .Last();

        var expectedTestSetModel = new TestSetModel
        {
            Id = testSet.Id,
            Inputs = [],
            IsPublic = testSet.IsPublic,
            Name = testSet.Name,
            ProblemId = testSet.Problem?.Id.AsString
        };

        foreach (var testSetInput in testSet.Inputs!)
        {
            expectedTestSetModel.Inputs.Add(new TestSetValueModel
            {
                DataType = testSetInput.DataType,
                Index = testSetInput.Index,
                IsArray = testSetInput.IsArray,
                ValueAsJson = testSetInput.ValueAsJson
            });
        }

        // Act
        var actualTestSetModel = testSet.ToModel();

        // Assert
        Assert.Equal(expectedTestSetModel, actualTestSetModel, new TestSetModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModel_For_TestSetInput_Should_Return_TestSetInputModel()
    {
        // Arrange
        var testSetInput = new TestSetValue
        {
            DataType = "Data Type",
            Index = 9999,
            IsArray = true,
            ValueAsJson = "ValueAsJson"
        };

        var expectedTestSetInputModel = new TestSetValueModel
        {
            DataType = testSetInput.DataType,
            Index = testSetInput.Index,
            IsArray = testSetInput.IsArray,
            ValueAsJson = testSetInput.ValueAsJson
        };

        // Act
        var actualTestSetInputModel = testSetInput.ToModel();

        // Assert
        Assert.Equal(expectedTestSetInputModel, actualTestSetInputModel, new TestSetValueModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModel_For_TestSetResult_Should_Return_TestSetResultModel()
    {
        // Arrange
        var testSetResult = new TestSetResult
        {
            Passed = true,
            RunDuration = new TimeSpan(0, 0, 5, 0),
            TestSet = new MongoDBRef("test-sets", "000000000000000000000010")
        };

        var expectedTestSetResultModel = new TestSetResultModel
        {
            Passed = testSetResult.Passed,
            RunDuration = testSetResult.RunDuration,
            TestSetId = testSetResult.TestSet?.Id.AsString
        };

        // Act
        var actualTestSetResultModel = testSetResult.ToModel();

        // Assert
        Assert.Equal(expectedTestSetResultModel, actualTestSetResultModel, new TestSetResultModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModel_For_User_Should_Return_UserModel()
    {
        // Arrange
        var usersTestData = new UsersTestData();

        var user = usersTestData
            .Where(userTestData => (UserDataIssues)userTestData[1] == UserDataIssues.None)
            .Select(userTestData => userTestData[0])
            .Cast<User>()
            .Last();

        var expectedUserModel = new UserModel
        {
            Id = user.Id,
            Role = user.Role,
            Team = user.Team != null
                ? new TeamModel
                {
                    CompetitionLevel = user.Team.CompetitionLevel.ToString(),
                    SchoolNumber = user.Team.SchoolNumber,
                    TeamNumber = user.Team.TeamNumber
                }
                : null,
            UserName = user.UserName
        };

        // Act
        var actualUserModel = user.ToModel();

        // Assert
        Assert.Equal(expectedUserModel, actualUserModel, new UserModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModels_For_Submission_Should_Return_SubmissionModel()
    {
        // Arrange
        var submissionsTestData = new SubmissionsTestData();

        var submissions = submissionsTestData
            .Where(submissionTestData => (SubmissionDataIssues)submissionTestData[1] == SubmissionDataIssues.None)
            .Select(submissionTestData => submissionTestData[0])
            .Cast<Submission>()
            .ToList();

        var expectedSubmissionModels = new List<SubmissionModel>();

        foreach (var submission in submissions)
        {
            var expectedSubmissionModel = new SubmissionModel
            {
                Id = submission.Id,
                IsFinalSubmission = submission.IsFinalSubmission,
                Language = submission.Language,
                ProblemId = submission.Problem?.Id.AsString,
                Solution = submission.Solution,
                SubmittedOn = submission.SubmittedOn,
                TestSetResults = new List<TestSetResultModel>(),
                User = new UserModel { Id = submission.User?.Id.AsString }
            };
            foreach (var submissionTestSetResult in submission.TestSetResults!)
            {
                expectedSubmissionModel.TestSetResults.Add(new TestSetResultModel
                {
                    Passed = submissionTestSetResult.Passed,
                    RunDuration = submissionTestSetResult.RunDuration,
                    TestSetId = submissionTestSetResult.TestSet?.Id.AsString
                });
            }

            expectedSubmissionModels.Add(expectedSubmissionModel);
        }

        // Act
        var actualSubmissionModels = Coding.WebApi.Entities.EntityExtensions.ToModels(submissions);

        // Assert
        Assert.Equal(expectedSubmissionModels, actualSubmissionModels, new SubmissionModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModels_For_TestSetResults_Should_Return_TestSetResultModels()
    {
        // Arrange
        var testSetResults = new List<TestSetResult>
        {
            new()
            {
                Passed = true,
                RunDuration = new TimeSpan(0, 0, 5, 0),
                TestSet = new MongoDBRef("test-sets", "000000000000000000000010")
            },
            new()
            {
                Passed = true,
                RunDuration = new TimeSpan(0, 0, 5, 0),
                TestSet = new MongoDBRef("test-sets", "000000000000000000000011")
            }
        };

        var expectedTestSetResultModels = new List<TestSetResultModel>
        {
            new()
            {
                Passed = testSetResults[0].Passed,
                RunDuration = testSetResults[0].RunDuration,
                TestSetId = testSetResults[0].TestSet?.Id.AsString
            },
            new()
            {
                Passed = testSetResults[1].Passed,
                RunDuration = testSetResults[1].RunDuration,
                TestSetId = testSetResults[1].TestSet?.Id.AsString
            }
        };

        // Act
        var actualTestSetResultModels = Coding.WebApi.Entities.EntityExtensions.ToModels(testSetResults);

        // Assert
        Assert.Equal(expectedTestSetResultModels, actualTestSetResultModels, new TestSetResultModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModels_For_TestSets_Should_Return_TestSetModels()
    {
        // Arrange
        var testSetsTestData = new TestSetsTestData();

        var testSets = testSetsTestData
            .Where(testSetTestData => (TestSetDataIssues)testSetTestData[1] == TestSetDataIssues.None)
            .Select(testSetTestData => testSetTestData[0])
            .Cast<TestSet>()
            .ToList();

        var expectedTestSetModels = new List<TestSetModel>
        {
            new()
            {
                Id = testSets[0].Id,
                Inputs = [],
                IsPublic = testSets[0].IsPublic,
                Name = testSets[0].Name,
                ProblemId = testSets[0].Problem?.Id.AsString
            },
            new()
            {
                Id = testSets[1].Id,
                Inputs = [],
                IsPublic = testSets[1].IsPublic,
                Name = testSets[1].Name,
                ProblemId = testSets[1].Problem?.Id.AsString
            }
        };

        for (var index = 0; index < testSets.Count; index++)
        {
            var testSet = testSets[index];
            var expectedTestSetModel = expectedTestSetModels[index];

            expectedTestSetModel.Inputs = new List<TestSetValueModel>();

            foreach (var testSetInput in testSet.Inputs!)
            {
                expectedTestSetModel.Inputs.Add(new TestSetValueModel
                {
                    DataType = testSetInput.DataType,
                    Index = testSetInput.Index,
                    IsArray = testSetInput.IsArray,
                    ValueAsJson = testSetInput.ValueAsJson
                });
            }
        }

        // Act
        var actualTestSetModels = Coding.WebApi.Entities.EntityExtensions.ToModels(testSets);

        // Assert
        Assert.Equal(expectedTestSetModels, actualTestSetModels, new TestSetModelEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void ToModels_For_Users_Should_Return_UserModels()
    {
        // Arrange
        var usersTestData = new UsersTestData();

        var users = usersTestData
            .Where(userTestData => (UserDataIssues)userTestData[1] == UserDataIssues.None)
            .Select(userTestData => userTestData[0])
            .Cast<User>()
            .ToList();

        var expectedUserModels = new List<UserModel>();

        foreach (var user in users)
        {
            var expectedUserModel = new UserModel
            {
                Id = user.Id,
                Role = user.Role,
                Team = user.Team != null
                    ? new TeamModel
                    {
                        CompetitionLevel = user.Team.CompetitionLevel.ToString(),
                        SchoolNumber = user.Team.SchoolNumber,
                        TeamNumber = user.Team.TeamNumber
                    }
                    : null,
                UserName = user.UserName
            };
            expectedUserModels.Add(expectedUserModel);
        }

        // Act
        var actualUserModels = Coding.WebApi.Entities.EntityExtensions.ToModels(users);

        // Assert
        Assert.Equal(expectedUserModels, actualUserModels, new UserModelEqualityComparer());
    }
}
