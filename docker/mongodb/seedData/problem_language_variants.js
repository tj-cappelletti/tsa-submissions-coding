db = db.getSiblingDB('tsa_submissions_coding');

db.problem_language_variants.createIndex(
    { problemId: 1, programmingLanguageId: 1, programmingLanguageVersionTag: 1 },
    { unique: true }
);

db.problem_language_variants.insertMany([
    {
        "_id": ObjectId("000000000000000000000001"),
        "baselineMetrics": {
            "executionTimeInMs": 0,
            "cyclomaticComplexity": 0,
            "linesOfCode": 0
        },
        "isActive": true,
        "problemId": ObjectId("000000000000000000000001"),
        "programmingLanguageId": ObjectId("000000000000000000000001"),
        "programmingLanguageVersionTag": "dotnet9.0",
        "referenceSolution": "namespace TsaCoding;\npublic class Program\n{\n    public static int FlipForBalance(string s)\n    {\n        int balance = 0;\n        int flips = 0;\n        foreach (char c in s)\n        {\n            if (c == '(')\n            {\n                balance++;\n            }\n            else if (c == ')')\n            {\n                balance--;\n            }\n            if (balance < 0)\n            {\n                flips++;\n                balance = 1; \/\/ Flip ')' to '('\n            }\n        }\n        flips += balance \/ 2; \/\/ Each pair of unmatched '(' can be flipped\n        return flips;\n    }\n}",
        "starterCode": "namespace TsaCoding;\npublic class Program\n{\n    public static int FlipForBalance(string s)\n    {\n    }\n}",
        "testHarnessCode": "using Xunit;\n\nnamespace TsaCoding;\npublic class TestFixture\n{\n    [Theory]\n    [InlineData(\"((\", 1)]\n    public void Test_FlipForBalance_Case_1(string s, int expected)\n    {\n        int result = Program.FlipForBalance(s);\n        Assert.Equal(expected, result);\n    }\n\n    [Theory]\n    [InlineData(\"(()\", 0)]\n    public void Test_FlipForBalance_Case_2(string s, int expected)\n    {\n        int result = Program.FlipForBalance(s);\n        Assert.Equal(expected, result);\n    }\n\n    [Theory]\n    [InlineData(\"))\", 1)]\n\n    public void Test_FlipForBalance_Case_3(string s, int expected)\n    {\n        int result = Program.FlipForBalance(s);\n        Assert.Equal(expected, result);\n    }\n\n    [Theory]\n    [InlineData(\"()\", 0)]\n    public void Test_FlipForBalance_Case_4(string s, int expected)\n    {\n        int result = Program.FlipForBalance(s);\n        Assert.Equal(expected, result);\n    }\n\n    [Theory]\n    [InlineData(\"((()\", 0)]\n    public void Test_FlipForBalance_Case_5(string s, int expected)\n    {\n        int result = Program.FlipForBalance(s);\n        Assert.Equal(expected, result);\n    }\n}",
        "workspaceFiles": [
            {
                "type": "File",
                "path": "Solution.csproj",
                "contents": "<Project Sdk=\"Microsoft.NET.Sdk\">\n  <PropertyGroup>\n    <OutputType>Exe<\/OutputType>\n    <TargetFramework>net9.0<\/TargetFramework>\n    <Nullable>enable<\/Nullable>\n  <\/PropertyGroup>\n  <ItemGroup>\n    <PackageReference Include=\"coverlet.collector\" Version=\"6.0.4\" \/>\n    <PackageReference Include=\"Microsoft.NET.Test.Sdk\" Version=\"17.14.1\" \/>\n    <PackageReference Include=\"xunit\" Version=\"2.9.3\" \/>\n    <PackageReference Include=\"xunit.runner.visualstudio\" Version=\"3.1.4\" \/>\n  <\/ItemGroup>\n  <ItemGroup>\n    <Using Include=\"Xunit\" \/>\n  <\/ItemGroup>\n<\/Project>",
                "isTemplate": false
            },
            {
                "type": "Link",
                "path": "Program.cs",
                "source": "submission"
            },
            {
                "type": "Link",
                "path": "TestFixture.cs",
                "source": "testHarnessCode"
            }
        ]
    }
]);
