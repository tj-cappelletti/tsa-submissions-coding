db = db.getSiblingDB('tsa_submissions_coding');

db.programming_languages.createIndex({ identifier: 1 }, { unique: true });
db.programming_languages.createIndex({ name: 1 }, { unique: true });

db.programming_languages.insertMany([
    {
        "_id": ObjectId("000000000000000000000001"),
        "fileExtension": ".cs",
        "identifier": "csharp",
        "isEnabled": true,
        "name": "C#",
        "versions": [
            {
                "displayName": ".NET 9.0",
                "isDefault": false,
                "versionTag": "dotnet9.0"
            },
            {
                "displayName": ".NET 10.0",
                "isDefault": true,
                "versionTag": "dotnet10.0"
            }
        ]
    }
]);
