using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.UnitTests.Data;

[ExcludeFromCodeCoverage]
public class UsersTestData : IEnumerable<object[]>
{
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return
        [
            new User
            {
                Id = "000000000000000000000001",
                Role = "judge",
                UserName = "judge01"
            },
            UserDataIssues.None
        ];

        yield return
        [
            new User
            {
                Id = "000000000000000000000002",
                Role = "participant",
                Team = new Team(CompetitionLevel.HighSchool, "9000", "901"),
                UserName = "9000-901"
            },
            UserDataIssues.None
        ];
    }
}

[Flags]
public enum UserDataIssues
{
    None = 0
}
