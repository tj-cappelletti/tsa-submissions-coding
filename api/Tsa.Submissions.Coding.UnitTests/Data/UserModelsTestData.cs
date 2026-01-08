using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.UnitTests.Data;

[ExcludeFromCodeCoverage]
public class UserModelsTestData : IEnumerable<object[]>
{
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return
        [
            new UserModel
            {
                Role = "judge",
                UserName = "judge01"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Role = "participant",
                Team = new TeamModel
                {
                    CompetitionLevel = "HighSchool",
                    SchoolNumber = "9000",
                    TeamNumber = "901"
                },
                UserName = "9000-901"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Role = "participant",
                Team = new TeamModel
                {
                    CompetitionLevel = "HighSchool",
                    SchoolNumber = "9000",
                    TeamNumber = "902"
                },
                UserName = "9000-902"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Role = "participant",
                Team = new TeamModel
                {
                    CompetitionLevel = "HighSchool",
                    SchoolNumber = "9000",
                    TeamNumber = "903"
                },
                UserName = "9000-903"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Role = "participant",
                Team = new TeamModel
                {
                    CompetitionLevel = "HighSchool",
                    SchoolNumber = "9001",
                    TeamNumber = "901"
                },
                UserName = "9001-901"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Role = "participant",
                Team = new TeamModel
                {
                    CompetitionLevel = "HighSchool",
                    SchoolNumber = "9001",
                    TeamNumber = "902"
                },
                UserName = "9001-902"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Role = "participant",
                Team = new TeamModel
                {
                    CompetitionLevel = "HighSchool",
                    SchoolNumber = "9001",
                    TeamNumber = "903"
                },
                UserName = "9001-903"
            },
            UserModelDataIssues.None
        ];

        yield return
        [
            new UserModel
            {
                Team = new TeamModel
                {
                    CompetitionLevel = "HighSchool",
                    SchoolNumber = "9000",
                    TeamNumber = "901"
                },
                UserName = "9000-901"
            },
            UserModelDataIssues.RoleNotValid
        ];

        yield return
        [
            new UserModel
            {
                Role = "dog",
                Team = new TeamModel
                {
                    CompetitionLevel = "HighSchool",
                    SchoolNumber = "9000",
                    TeamNumber = "901"
                },
                UserName = "9000-901"
            },
            UserModelDataIssues.RoleNotValid
        ];

        yield return
        [
            new UserModel
            {
                Role = "participant",
                UserName = "9000-901"
            },
            UserModelDataIssues.ParticipantMissingTeam
        ];

        yield return
        [
            new UserModel
            {
                Role = "participant",
                Team = new TeamModel
                {
                    SchoolNumber = "9000",
                    TeamNumber = "901"
                },
                UserName = ""
            },
            UserModelDataIssues.UserNameNotValid
        ];
    }
}

[Flags]
public enum UserModelDataIssues
{
    None = 0,
    RoleNotValid = 1 << 1,
    ParticipantMissingTeam = 1 << 2,
    UserNameNotValid = 1 << 3
}
