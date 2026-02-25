using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.UnitTests.Data;

[ExcludeFromCodeCoverage]
internal class ProgrammingLanguagesTestData : IEnumerable<object[]>
{
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return
        [
            new ProgrammingLanguage
            {
                FileExtension = ".cs",
                Id = "000000000000000000000001",
                Identifier = "csharp",
                IsEnabled = true,
                Name = "C#",
                Versions =
                [
                    new ProgrammingLanguageVersion
                    {
                        DisplayName = ".NET 9.0",
                        IsDefault = false,
                        VersionTag = "dotnet9.0"
                    },
                    new ProgrammingLanguageVersion
                    {
                        DisplayName = ".NET 10.0",
                        IsDefault = false,
                        VersionTag = "dotnet10.0"
                    }
                ]
            },
            ProgrammingLanguageDataIssues.None
        ];

        yield return
        [
            new ProgrammingLanguage
            {
                FileExtension = ".java",
                Id = "000000000000000000000002",
                Identifier = "java",
                IsEnabled = true,
                Name = "Java",
                Versions =
                [
                    new ProgrammingLanguageVersion
                    {
                        DisplayName = "Java 17",
                        IsDefault = false,
                        VersionTag = "java17"
                    },
                    new ProgrammingLanguageVersion
                    {
                        DisplayName = "Java 21",
                        IsDefault = true,
                        VersionTag = "java21"
                    }
                ]
            },
            ProgrammingLanguageDataIssues.None
        ];
    }
}

[Flags]
public enum ProgrammingLanguageDataIssues
{
    None = 0
}
