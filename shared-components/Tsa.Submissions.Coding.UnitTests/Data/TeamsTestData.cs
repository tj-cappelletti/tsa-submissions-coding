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
            new Team(CompetitionLevel.HighSchool, "2000", "901"),
            TeamDataIssues.None
        ];

        yield return
        [
            new Team(CompetitionLevel.HighSchool, "2000", "902"),
            TeamDataIssues.None
        ];


        yield return
        [
            new Team(CompetitionLevel.HighSchool, "2000", "903"),
            TeamDataIssues.None
        ];


        yield return
        [
            new Team(CompetitionLevel.MiddleSchool, "1001", "901"),
            TeamDataIssues.None
        ];


        yield return
        [
            new Team(CompetitionLevel.MiddleSchool, "1001", "902"),
            TeamDataIssues.None
        ];


        yield return
        [
            new Team(CompetitionLevel.MiddleSchool, "1001", "903"),
            TeamDataIssues.None
        ];

        yield return
        [
            new Team(CompetitionLevel.MiddleSchool, "dog", "901"),
            TeamDataIssues.InvalidSchoolNumber
        ];

        yield return
        [
            new Team(CompetitionLevel.MiddleSchool, "1001", "bird"),
            TeamDataIssues.InvalidCompetitionLevel | TeamDataIssues.InvalidSchoolNumber | TeamDataIssues.InvalidTeamNumber
        ];

        yield return
        [
            new Team(CompetitionLevel.MiddleSchool, "dog", "bird"),
            TeamDataIssues.InvalidCompetitionLevel | TeamDataIssues.InvalidSchoolNumber | TeamDataIssues.InvalidTeamNumber
        ];

        yield return
        [
            new Team(CompetitionLevel.MiddleSchool, "9999", "901"),
            TeamDataIssues.InvalidCompetitionLevel | TeamDataIssues.InvalidSchoolNumber
        ];

        yield return
        [
            new Team(CompetitionLevel.MiddleSchool, "1001", "bird"),
            TeamDataIssues.InvalidCompetitionLevel | TeamDataIssues.InvalidSchoolNumber | TeamDataIssues.InvalidTeamNumber
        ];

        yield return
        [
            new Team(CompetitionLevel.MiddleSchool, "dog", "bird"),
            TeamDataIssues.InvalidCompetitionLevel | TeamDataIssues.InvalidSchoolNumber | TeamDataIssues.InvalidTeamNumber
        ];

        yield return
        [
            new Team(CompetitionLevel.MiddleSchool, "9999", "901"),
            TeamDataIssues.InvalidCompetitionLevel | TeamDataIssues.InvalidSchoolNumber
        ];

        yield return
        [
            new Team(CompetitionLevel.MiddleSchool, "9001", "902"),
            TeamDataIssues.InvalidCompetitionLevel | TeamDataIssues.InvalidSchoolNumber | TeamDataIssues.InvalidTeamNumber
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
