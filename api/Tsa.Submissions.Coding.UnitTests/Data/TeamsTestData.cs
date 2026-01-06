using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.UnitTests.Data;

[ExcludeFromCodeCoverage]
public class TeamsTestData : IEnumerable<object[]>
{
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return
        [
            new Team
            {
                CompetitionLevel = CompetitionLevel.HighSchool,
                SchoolNumber = "2000",
                TeamNumber = "901"
            },
            TeamDataIssues.None
        ];

        yield return
        [
            new Team
            {
                CompetitionLevel = CompetitionLevel.HighSchool,
                SchoolNumber = "2000",
                TeamNumber = "902"
            },
            TeamDataIssues.None
        ];


        yield return
        [
            new Team
            {
                CompetitionLevel = CompetitionLevel.HighSchool,
                SchoolNumber = "2000",
                TeamNumber = "903"
            },
            TeamDataIssues.None
        ];


        yield return
        [
            new Team
            {
                CompetitionLevel = CompetitionLevel.MiddleSchool,
                SchoolNumber = "1001",
                TeamNumber = "901"
            },
            TeamDataIssues.None
        ];


        yield return
        [
            new Team
            {
                CompetitionLevel = CompetitionLevel.MiddleSchool,
                SchoolNumber = "1001",
                TeamNumber = "902"
            },
            TeamDataIssues.None
        ];


        yield return
        [
            new Team
            {
                CompetitionLevel = CompetitionLevel.MiddleSchool,
                SchoolNumber = "1001",
                TeamNumber = "903"
            },
            TeamDataIssues.None
        ];

        yield return
        [
            new Team
            {
                CompetitionLevel = CompetitionLevel.MiddleSchool,
                SchoolNumber = "dog",
                TeamNumber = "901"
            },
            TeamDataIssues.InvalidSchoolNumber
        ];

        yield return
        [
            new Team
            {
                SchoolNumber = "dog",
                TeamNumber = "101"
            },
            TeamDataIssues.InvalidCompetitionLevel | TeamDataIssues.InvalidSchoolNumber | TeamDataIssues.InvalidTeamNumber
        ];

        yield return
        [
            new Team
            {
                SchoolNumber = "dog",
                TeamNumber = "bird"
            },
            TeamDataIssues.InvalidCompetitionLevel | TeamDataIssues.InvalidSchoolNumber | TeamDataIssues.InvalidTeamNumber
        ];

        yield return
        [
            new Team
            {
                SchoolNumber = "9999",
                TeamNumber = "901",
            },
            TeamDataIssues.InvalidCompetitionLevel | TeamDataIssues.InvalidSchoolNumber
        ];

        yield return
        [
            new Team
            {
                SchoolNumber = "9001",
                TeamNumber = "902"
            },
            TeamDataIssues.InvalidCompetitionLevel | TeamDataIssues.InvalidCompetitionLevel | TeamDataIssues.InvalidSchoolNumber
        ];
    }
}

[Flags]
public enum TeamDataIssues
{
    None = 0,
    InvalidCompetitionLevel = 1 << 0,
    InvalidSchoolNumber = 1 << 1,
    InvalidTeamNumber = 1 << 2
}
