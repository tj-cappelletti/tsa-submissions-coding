using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.UnitTests.Data;

[ExcludeFromCodeCoverage]
internal class SubmissionsTestData : IEnumerable<object[]>
{
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return
        [
            new Submission
            {
                EvaluatedOn = null,
                Id = "000000000000000000000001",
                ProblemId = "000000000000000000000001",
                ProgrammingLanguageId = "000000000000000000000001",
                ProgrammingLanguageVersionTag = "dotnet9.0",
                Scorecard = null,
                Solution = "Console.WriteLine(\"Hello, World!\");",
                SubmittedOn = DateTimeOffset.UtcNow.Subtract(TimeSpan.FromMinutes(5)),
                TestCaseResults = [],
                UserId = "000000000000000000000002"
            },
            SubmissionDataIssues.None
        ];

        yield return
        [
            new Submission
            {
                EvaluatedOn = null,
                Id = "000000000000000000000002",
                ProblemId = "000000000000000000000001",
                ProgrammingLanguageId = "000000000000000000000001",
                ProgrammingLanguageVersionTag = "dotnet9.0",
                Scorecard = null,
                Solution = "Console.WriteLine(\"Hello TSA!!\");",
                SubmittedOn = DateTimeOffset.UtcNow.Subtract(TimeSpan.FromMinutes(5)),
                TestCaseResults = [],
                UserId = "000000000000000000000003"
            },
            SubmissionDataIssues.None
        ];

        yield return
        [
            new Submission
            {
                EvaluatedOn = null,
                Id = "000000000000000000000003",
                ProblemId = "000000000000000000000001",
                ProgrammingLanguageId = "000000000000000000000002",
                ProgrammingLanguageVersionTag = "java17",
                Scorecard = null,
                Solution = "System.out.println(\"Hello, World!\");",
                SubmittedOn = DateTimeOffset.UtcNow.Subtract(TimeSpan.FromMinutes(5)),
                TestCaseResults = [],
                UserId = "000000000000000000000004"
            },
            SubmissionDataIssues.None
        ];
    }
}

[Flags]
public enum SubmissionDataIssues
{
    None = 0
}
