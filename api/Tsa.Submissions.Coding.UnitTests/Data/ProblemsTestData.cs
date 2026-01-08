using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.UnitTests.Data;

[ExcludeFromCodeCoverage]
public class ProblemsTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return
        [
            new Problem
            {
                Description = "This is the description of Problem #1",
                Id = "000000000000000000000001",
                IsActive = true,
                Title = "Problem #1"
            },
            ProblemDataIssues.None
        ];

        yield return
        [
            new Problem
            {
                Description = "This is the description of Problem #2",
                Id = "000000000000000000000002",
                IsActive = true,
                Title = "Problem #2"
            },
            ProblemDataIssues.None
        ];

        yield return
        [
            new Problem
            {
                Description = "This is the description of Problem #3",
                Id = "000000000000000000000003",
                IsActive = true,
                Title = "Problem #3"
            },
            ProblemDataIssues.None
        ];

        yield return
        [
            new Problem
            {
                Description = "This is the description of Problem #4",
                Id = "000000000000000000000004",
                IsActive = false,
                Title = "Problem #4"
            },
            ProblemDataIssues.None
        ];

        yield return
        [
            new Problem
            {
                Id = "000000000000000000000005",
                IsActive = false,
                Title = "Problem #5"
            },
            ProblemDataIssues.MissingDescription
        ];

        yield return
        [
            new Problem
            {
                Description = "This is the description of Problem #6",
                Id = "000000000000000000000006",
                IsActive = false,
            },
            ProblemDataIssues.MissingTitle
        ];

        yield return
        [
            new Problem
            {
                Id = "000000000000000000000007",
                IsActive = false,
            },
            ProblemDataIssues.MissingDescription | ProblemDataIssues.MissingTitle
        ];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

[Flags]
public enum ProblemDataIssues
{
    None = 0,
    MissingDescription = 1 << 0,
    MissingTitle = 1 << 1
}