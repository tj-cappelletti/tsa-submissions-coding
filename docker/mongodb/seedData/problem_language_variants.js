db = db.getSiblingDB('tsa_submissions_coding');

db.problem_language_variants.createIndex(
    { problemId: 1, programmingLanguageId: 1, programmingLanguageVersionTag: 1 },
    { unique: true }
);

db.problem_language_variants.insertMany([
    {
        "_id": ObjectId("000000000000000000000001"),
        "isActive": true,
        "programmingLanguageId": ObjectId("000000000000000000000001"),
        "problemId": ObjectId("000000000000000000000001"),
        "referenceSolution": "namespace TsaCoding;\r\npublic class Program\r\n{\r\n    public static int FlipForBalance(string s)\r\n    {\r\n        int balance = 0;\r\n        int flips = 0;\r\n        foreach (char c in s)\r\n        {\r\n            if (c == '(')\r\n            {\r\n                balance++;\r\n            }\r\n            else if (c == ')')\r\n            {\r\n                balance--;\r\n            }\r\n            if (balance < 0)\r\n            {\r\n                flips++;\r\n                balance = 1; \/\/ Flip ')' to '('\r\n            }\r\n        }\r\n        flips += balance \/ 2; \/\/ Each pair of unmatched '(' can be flipped\r\n        return flips;\r\n    }\r\n}",
        "starterCode": "namespace TsaCoding;\r\npublic class Program\r\n{\r\n    public static int FlipForBalance(string s)\r\n    {\r\n    }\r\n}",
        "testHarnessCode": "using Xunit;\r\n\r\nnamespace TsaCoding;\r\npublic class TestFixture\r\n{\r\n    [Theory]\r\n    [InlineData(\"((\", 1)]\r\n    public void Test_FlipForBalance_Case_1(string s, int expected)\r\n    {\r\n        int result = Program.FlipForBalance(s);\r\n        Assert.Equal(expected, result);\r\n    }\r\n\r\n    [Theory]\r\n    [InlineData(\"(()\", 0)]\r\n    public void Test_FlipForBalance_Case_2(string s, int expected)\r\n    {\r\n        int result = Program.FlipForBalance(s);\r\n        Assert.Equal(expected, result);\r\n    }\r\n\r\n    [Theory]\r\n    [InlineData(\"))\", 1)]\r\n\r\n    public void Test_FlipForBalance_Case_3(string s, int expected)\r\n    {\r\n        int result = Program.FlipForBalance(s);\r\n        Assert.Equal(expected, result);\r\n    }\r\n\r\n    [Theory]\r\n    [InlineData(\"()\", 0)]\r\n    public void Test_FlipForBalance_Case_4(string s, int expected)\r\n    {\r\n        int result = Program.FlipForBalance(s);\r\n        Assert.Equal(expected, result);\r\n    }\r\n\r\n    [Theory]\r\n    [InlineData(\"((()\", 0)]\r\n    public void Test_FlipForBalance_Case_5(string s, int expected)\r\n    {\r\n        int result = Program.FlipForBalance(s);\r\n        Assert.Equal(expected, result);\r\n    }\r\n}",
        "programmingLanguageVersionTag": ["dotnet9.0"]
    }
]);
