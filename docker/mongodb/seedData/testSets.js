db = db.getSiblingDB('tsa_submissions_coding');

db.test_sets.insertMany([
    {
        "_id": ObjectId("000000000000000000000001"),
        "inputs": [
            {
                "dataType": "string",
                "index": 0,
                "isArray": false,
                "valueAsJson": "{ \"value\": \"((\" }"
            }
        ],
        "isPublic": true,
        "name": "Filp for Balance - Input #1",
        "problem": {
            "$ref": "problems",
            "$id": "000000000000000000000001",
            "$db": ""
        }
    },
    {
        "_id": ObjectId("000000000000000000000002"),
        "inputs": [
            {
                "dataType": "string",
                "index": 0,
                "isArray": false,
                "valueAsJson": "{ \"value\": \"))\" }"
            }
        ],
        "isPublic": true,
        "name": "Filp for Balance - Input #2",
        "problem": {
            "$ref": "problems",
            "$id": "000000000000000000000001",
            "$db": ""
        }
    },
    {
        "_id": ObjectId("000000000000000000000003"),
        "inputs": [
            {
                "dataType": "string",
                "index": 0,
                "isArray": false,
                "valueAsJson": "{ \"value\": \"()\" }"
            }
        ],
        "isPublic": true,
        "name": "Filp for Balance - Input #3",
        "problem": {
            "$ref": "problems",
            "$id": "000000000000000000000001",
            "$db": ""
        }
    },
    {
        "_id": ObjectId("000000000000000000000004"),
        "inputs": [
            {
                "dataType": "string",
                "index": 0,
                "isArray": false,
                "valueAsJson": "{ \"value\": \"((()\" }"
            }
        ],
        "isPublic": true,
        "name": "Filp for Balance - Input #4",
        "problem": {
            "$ref": "problems",
            "$id": "000000000000000000000001",
            "$db": ""
        }
    },
    {
        "_id": ObjectId("000000000000000000000005"),
        "inputs": [
            {
                "dataType": "string",
                "index": 0,
                "isArray": false,
                "valueAsJson": "{ \"value\": \")))(\" }"
            }
        ],
        "isPublic": true,
        "name": "Filp for Balance - Input #5",
        "problem": {
            "$ref": "problems",
            "$id": "000000000000000000000001",
            "$db": ""
        }
    },
    {
        "_id": ObjectId("000000000000000000000006"),
        "inputs": [
            {
                "dataType": "string",
                "index": 0,
                "isArray": true,
                "valueAsJson": "{ \"value\": [ \"qwertyuiop\", \"asdfghjkl\", \"^zxcvbnm \" ] }"
            },
            {
                "dataType": "string",
                "index": 1,
                "isArray": false,
                "valueAsJson": "{ \"value\": \"qwerty\" }"
            },
        ],
        "isPublic": true,
        "name": "Typing Distance - Input #1",
        "problem": {
            "$ref": "problems",
            "$id": "000000000000000000000002",
            "$db": ""
        }
    },
    {
        "_id": ObjectId("000000000000000000000007"),
        "inputs": [
            {
                "dataType": "string",
                "index": 0,
                "isArray": true,
                "valueAsJson": "{ \"value\": [ \"qwertyuiop\", \"asdfghjkl\", \"^zxcvbnm \" ] }"
            },
            {
                "dataType": "string",
                "index": 1,
                "isArray": false,
                "valueAsJson": "{ \"value\": \"Qwerty\" }"
            },
        ],
        "isPublic": true,
        "name": "Typing Distance - Input #2",
        "problem": {
            "$ref": "problems",
            "$id": "000000000000000000000002",
            "$db": ""
        }
    },
    {
        "_id": ObjectId("000000000000000000000008"),
        "inputs": [
            {
                "dataType": "string",
                "index": 0,
                "isArray": true,
                "valueAsJson": "{ \"value\": [ \"qwertyuiop\", \"asdfghjkl\", \"^zxcvbnm \" ] }"
            },
            {
                "dataType": "string",
                "index": 1,
                "isArray": false,
                "valueAsJson": "{ \"value\": \"abc\" }"
            },
        ],
        "isPublic": true,
        "name": "Typing Distance - Input #3",
        "problem": {
            "$ref": "problems",
            "$id": "000000000000000000000002",
            "$db": ""
        }
    }
])