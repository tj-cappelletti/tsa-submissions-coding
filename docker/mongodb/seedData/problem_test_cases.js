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
        "signature": "sha256:84d1960892df94117dd6ae1d3c185fc3b57f9b5468f8418b3eb1374e017326c7"
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
        "signature": "sha256:168334ac1fdfd64cba7693cc146876f1fd07e03c01e15533029981c85a25c325"
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
        "signature": "sha256:84cabe36e97b2441c38c6624909791af535a67414cf648ad18c352c10894b916"
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
        "signature": "sha256:dfd16a00a9ad6eed9f68e8dc142a3a1184ef6c432ba78e1947686db3049d7e0c"
    }
]);