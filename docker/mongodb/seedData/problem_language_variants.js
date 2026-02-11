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
        "starterCode": "",
        "testHarnessCode": "",
        "programmingLanguageVersionTag": "dotnet10.0"
    }
]);
