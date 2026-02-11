db = db.getSiblingDB('tsa_submissions_coding');

db.problem_test_cases.createIndex({ problemId: 1, signature: 1 }, { unique: true });

db.problem_test_cases.insertMany([
    {
        "_id": ObjectId("000000000000000000000001"),
        "expectedOutput": "0",
        "inputs": [
            {
                "dataType": "String",
                "isArray": false,
                "index": 0,
                "value": "(()"
            }
        ],
        "isActive": true,
        "isPublic": true,
        "name": "Test Case 1",
        "outputDataType": "Int32",
        "outputIsArray": false,
        "problemId": ObjectId("000000000000000000000001"),
        "signature": ""
    },
    {
        "_id": ObjectId("000000000000000000000002"),
        "expectedOutput": "0",
        "inputs": [
            {
                "dataType": "String",
                "isArray": false,
                "index": 0,
                "value": "))"
            }
        ],
        "isActive": true,
        "isPublic": true,
        "name": "Test Case 2",
        "outputDataType": "Int32",
        "outputIsArray": false,
        "problemId": ObjectId("000000000000000000000001"),
        "signature": ""
    },
    {
        "_id": ObjectId("000000000000000000000003"),
        "expectedOutput": "0",
        "inputs": [
            {
                "dataType": "String",
                "isArray": false,
                "index": 0,
                "value": "()"
            }
        ],
        "isActive": true,
        "isPublic": true,
        "name": "Test Case 3",
        "outputDataType": "Int32",
        "outputIsArray": false,
        "problemId": ObjectId("000000000000000000000001"),
        "signature": ""
    },
    {
        "_id": ObjectId("000000000000000000000004"),
        "expectedOutput": "1",
        "inputs": [
            {
                "dataType": "String",
                "isArray": false,
                "index": 0,
                "value": "((()"
            }
        ],
        "isActive": true,
        "isPublic": true,
        "name": "Test Case 4",
        "outputDataType": "Int32",
        "outputIsArray": false,
        "problemId": ObjectId("000000000000000000000001"),
        "signature": ""
    }
]);