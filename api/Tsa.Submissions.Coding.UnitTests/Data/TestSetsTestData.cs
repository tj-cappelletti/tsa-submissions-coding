using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MongoDB.Driver;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.UnitTests.Data;

[ExcludeFromCodeCoverage]
public class TestSetsTestData : IEnumerable<object[]>
{
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return
        [
            new TestSet
            {
                Id = "000000000000000000000001",
                Inputs =
                [
                    new TestSetValue
                    {
                        DataType = "character",
                        Index = 0,
                        ValueAsJson = "{ \"value\": \"a\" }"
                    },
                    new TestSetValue
                    {
                        DataType = "character",
                        Index = 1,
                        IsArray = true,
                        ValueAsJson = "{ \"value\": [ \"a\", \"b\", \"c\" ] }"
                    },
                    new TestSetValue
                    {
                        DataType = "decimal",
                        Index = 2,
                        ValueAsJson = "{ \"value\": 1.1 }"
                    },
                    new TestSetValue
                    {
                        DataType = "decimal",
                        Index = 3,
                        IsArray = true,
                        ValueAsJson = "{ \"value\": [ 1.1, 1.2, 1.3 ] }"
                    },
                    new TestSetValue
                    {
                        DataType = "number",
                        Index = 4,
                        ValueAsJson = "{ \"value\": 1 }"
                    },
                    new TestSetValue
                    {
                        DataType = "number",
                        Index = 5,
                        IsArray = true,
                        ValueAsJson = "{ \"value\": [ 1, 2, 3 ] }"
                    },
                    new TestSetValue
                    {
                        DataType = "string",
                        Index = 6,
                        ValueAsJson = "{ \"value\": \"string a\" }"
                    },
                    new TestSetValue
                    {
                        DataType = "string",
                        Index = 7,
                        IsArray = true,
                        ValueAsJson = "{ \"value\": [ \"string a\", \"string b\", \"string c\" ] }"
                    }
                ],
                IsPublic = true,
                Name = "Test Set #1",
                Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, "000000000000000000000001")
            },
            TestSetDataIssues.None
        ];

        yield return
        [
            new TestSet
            {
                Id = "000000000000000000000002",
                Inputs =
                [
                    new TestSetValue
                    {
                        DataType = "string",
                        Index = 0,
                        ValueAsJson = "{ \"value\": \"test value\" }"
                    },
                    new TestSetValue
                    {
                        DataType = "number",
                        Index = 1,
                        ValueAsJson = "{ \"value\": 1 }"
                    }
                ],
                IsPublic = false,
                Name = "Test Set #2",
                Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, "000000000000000000000001")
            },
            TestSetDataIssues.None
        ];

        yield return
        [
            new TestSet
            {
                Id = "000000000000000000000003",
                Inputs =
                [
                    new TestSetValue
                    {
                        DataType = "string",
                        Index = 0,
                        ValueAsJson = "{ \"value\": \"test value\" }"
                    },
                    new TestSetValue
                    {
                        DataType = "number",
                        Index = 1,
                        ValueAsJson = "{ \"value\": 1 }"
                    }
                ],
                IsPublic = true,
                Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, "000000000000000000000002")
            },
            TestSetDataIssues.MissingName
        ];

        yield return
        [
            new TestSet
            {
                Id = "000000000000000000000004",
                Inputs =
                [
                    new TestSetValue
                    {
                        DataType = "string",
                        Index = 0,
                        ValueAsJson = "{ \"value\": \"test value\" }"
                    },
                    new TestSetValue
                    {
                        DataType = "number",
                        Index = 1,
                        ValueAsJson = "{ \"value\": 1 }"
                    }
                ],
                IsPublic = true,
                Name = "Test Set #4"
            },
            TestSetDataIssues.MissingProblemId
        ];

        yield return
        [
            new TestSet
            {
                Id = "000000000000000000000005",
                IsPublic = true,
                Name = "Test Set #5",
                Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, "000000000000000000000002")
            },
            TestSetDataIssues.MissingInput
        ];

        yield return
        [
            new TestSet
            {
                Id = "000000000000000000000006",
                Inputs =
                [
                    new TestSetValue
                    {
                        Index = 0,
                        ValueAsJson = "{ \"value\": \"test value\" }"
                    },
                    new TestSetValue
                    {
                        Index = 1,
                        ValueAsJson = "{ \"value\": 1 }"
                    }
                ],
                IsPublic = true,
                Name = "Test Set #6",
                Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, "000000000000000000000003")
            },
            TestSetDataIssues.MissingDataType
        ];

        yield return
        [
            new TestSet
            {
                Id = "000000000000000000000007",
                Inputs =
                [
                    new TestSetValue
                    {
                        DataType = "dog",
                        Index = 0,
                        ValueAsJson = "{ \"value\": \"test value\" }"
                    },
                    new TestSetValue
                    {
                        DataType = "cat",
                        Index = 1,
                        ValueAsJson = "{ \"value\": 1 }"
                    }
                ],
                IsPublic = true,
                Name = "Test Set #7",
                Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, "000000000000000000000003")
            },
            TestSetDataIssues.InvalidDataType
        ];

        yield return
        [
            new TestSet
            {
                Id = "000000000000000000000008",
                Inputs =
                [
                    new TestSetValue
                    {
                        DataType = "string",
                        ValueAsJson = "{ \"value\": \"test value\" }"
                    },
                    new TestSetValue
                    {
                        DataType = "number",
                        ValueAsJson = "{ \"value\": 1 }"
                    }
                ],
                IsPublic = true,
                Name = "Test Set #8",
                Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, "000000000000000000000003")
            },
            TestSetDataIssues.MissingIndex
        ];

        yield return
        [
            new TestSet
            {
                Id = "000000000000000000000009",
                Inputs =
                [
                    new TestSetValue
                    {
                        DataType = "string",
                        Index = 0,
                        ValueAsJson = "{ \"value\": \"test value\" }"
                    },
                    new TestSetValue
                    {
                        DataType = "number",
                        Index = 0,
                        ValueAsJson = "{ \"value\": 1 }"
                    }
                ],
                IsPublic = true,
                Name = "Test Set #2",
                Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, "000000000000000000000003")
            },
            TestSetDataIssues.IndexNotUnique
        ];

        yield return
        [
            new TestSet
            {
                Id = "000000000000000000000010",
                Inputs =
                [
                    new TestSetValue
                    {
                        DataType = "string",
                        Index = 0
                    },
                    new TestSetValue
                    {
                        DataType = "number",
                        Index = 1
                    }
                ],
                IsPublic = true,
                Name = "Test Set #10",
                Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, "000000000000000000000003")
            },
            TestSetDataIssues.MissingValueAsJson
        ];

        yield return
        [
            new TestSet
            {
                Id = "000000000000000000000011",
                Inputs =
                [
                    new TestSetValue
                    {
                        DataType = "number",
                        Index = 0,
                        ValueAsJson = "{ \"value\": \"test value\" }"
                    },
                    new TestSetValue
                    {
                        DataType = "number",
                        Index = 1,
                        ValueAsJson = "{ \"value\": 1 }"
                    }
                ],
                IsPublic = true,
                Name = "Test Set #11",
                Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, "000000000000000000000003")
            },
            TestSetDataIssues.ValueDoesNotMatchDataType
        ];

        yield return
        [
            new TestSet
            {
                Id = "000000000000000000000012",
                Inputs =
                [
                    new TestSetValue
                    {
                        DataType = "string",
                        Index = 0,
                        ValueAsJson = "Try me"
                    },
                    new TestSetValue
                    {
                        DataType = "number",
                        Index = 1,
                        ValueAsJson = "Try me"
                    }
                ],
                IsPublic = true,
                Name = "Test Set #12",
                Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, "000000000000000000000003")
            },
            TestSetDataIssues.ValueCannotBeDeserialized
        ];

        yield return
        [
            new TestSet
            {
                Id = "000000000000000000000012",
                Inputs =
                [
                    new TestSetValue
                    {
                        DataType = "string",
                        Index = 0,
                        ValueAsJson = "{ \"value\": \"test value\" }"
                    },
                    new TestSetValue
                    {
                        DataType = "number",
                        Index = 2,
                        ValueAsJson = "{ \"value\": 1 }"
                    }
                ],
                IsPublic = true,
                Name = "Test Set #2",
                Problem = new MongoDBRef(ProblemsService.MongoDbCollectionName, "000000000000000000000003")
            },
            TestSetDataIssues.IndexNotContinuous
        ];
    }
}

[Flags]
public enum TestSetDataIssues
{
    None = 0,
    MissingName = 1 << 0,
    MissingProblemId = 1 << 1,
    MissingInput = 1 << 2,
    MissingDataType = 1 << 3,
    InvalidDataType = 1 << 4,
    MissingIndex = 1 << 5,
    IndexNotUnique = 1 << 6,
    MissingValueAsJson = 1 << 7,
    ValueDoesNotMatchDataType = 1 << 8,
    ValueCannotBeDeserialized = 1 << 9,
    IndexNotContinuous = 1 << 10
}
