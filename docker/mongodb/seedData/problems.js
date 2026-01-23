db = db.getSiblingDB('tsa_submissions_coding');

db.problems.insertMany([
    {
        "description": "# Flip for Balance\r\nGiven a string s containing only open and close parenthesis. What is the minimum number of parenthesis that needs to be flipped for the string to become a set of balanced parentheses.\r\n\r\n## Example\r\n\r\n`For s = \"((\"` - the output should be `flipForBalance(s) = 1`.\r\n\r\n## Parameters\r\n|Input|Summary|Constraints|\r\n|-----|-------|-----------|\r\n|`string s`|A string of even non-empty length containing only '(' and ')'.|2 \u2264 `s.length` < 50.\r\n\r\n## Output\r\n`integer` - The minimum number of parenthesis that needs to be flipped.\r\n\r\n## Test Sets\r\n| `s` | Output |\r\n|:-----:|:------:|\r\n|((|1|\r\n|))|1|\r\n|()|0|\r\n|((()|1|\r\n|)))(|3|",
        "_id": ObjectId("000000000000000000000001"),
        "isActive": true,
        "title": "Flip for Balance",
        "testCases": [
            {
                "expectedOutput": "1",
                "input": "(( ",
                "isActive": true
            },
            {
                "expectedOutput": "1",
                "input": ")) ",
                "isActive": true
            },
            {
                "expectedOutput": "0",
                "input": "() ",
                "isActive": true,
            },
            {
                "expectedOutput": "1",
                "input": "((() ",
                "isActive": true,
            },
        ]
    },
    {
        "description": "# Typing Distance\r\nProvided a simple custom keyboard layout, your mission is to calculate the minimum total distance key stroke of a given message. The distance is calculated between each key is only in 4 directions - up, down, left and right. And you can't move to empty space.\r\n\r\n## Keyboard Layout\r\n- There is only one `[Shift]` button, presented as `[^]`. To keep the problem more relax the [Shift] is working once at a time. You press [Shift] and the next letter will be capitalized.\r\n- There is only one `[Spacebar]` button, present as `[ ]`\r\n- Leters are represented as themselves. Example: `a` will be `a`, `b` will be `b`, etc.\r\n\r\n## Parameters\r\n|Input|Summary|Constraints|\r\n|-----|-------|-----------|\r\n|string[] keyboard|Layout of the custom keyboard| - 0 < `keyboard.Length` < 30, `keyboard[i]` \u2208 `[a-z^ ]`|\r\n|string message|The message to find the path for|0 \u2264 `message.length` \u2264 10^3, `message[i]` \u2208 `[a-zA-Z ]`|\r\n\r\n## Output\r\n`integer` - The minimum typing distance for the given message and keyboard layout.\r\n\r\n## Examples\r\n### Example 1\r\nGiven the following:\r\n```\r\nkeyboard = [\"qwertyuiop\",\"asdfghjkl\",\"^zxcvbnm \"]\r\nmessage = \"qwerty\"\r\n```\r\nThe output should be:\r\n```\r\ntypingDistance(keyboard, message) = 5\r\n```\r\n\r\n- The minimum distance between `[q]` and `[w]` is 1.\r\n- The minimum distance between `[w]` and `[e]` is 1.\r\n- The minimum distance between `[e]` and `[r]` is 1.\r\n- The minimum distance between `[r]` and `[t]` is 1.\r\n- The minimum distance between `[t]` and `[y]` is 1.\r\n\r\nThis gives us a total of 5.\r\n\r\n### Example 2\r\nGiven the following:\r\n```\r\nkeyboard = [\"qwertyuiop\",\"asdfghjkl\",\"^zxcvbnm \"]\r\nmessage = \"Qwerty\"\r\n```\r\nThe output should be:\r\n```\r\ntypingDistance(keyboard, message) = 7\r\n```\r\n\r\nYou need to press the `[Shift]` key to get a capital `Q`. The minimum distance between `[Shift]` and `[q]` is 2.\r\n\r\n### Example 3\r\nGiven the following:\r\n```\r\nkeyboard = [\"qwertyuiop\",\"asdfghjkl\",\"^zxcvbnm \"]\r\nmessage = \"abc\"\r\n```\r\nThe output should be:\r\n```\r\ntypingDistance(keyboard, message) = 8\r\n```\r\nFrom `[a]`, you need to go to `[h]` before getting to `[b]`. You can't go from `[g]` to `[b]` directly.\r\n\r\n\r\n## Test Sets\r\n| `keyboard` | `message` | Output |\r\n|:----------:|:---------:|:------:|\r\n|`[\"qwertyuiop\",\"asdfghjkl\",\"^zxcvbnm \"]`|`\"qwerty\"`|5\r\n|`[\"qwertyuiop\",\"asdfghjkl\",\"^zxcvbnm \"]`|`\"Qwerty\"`|7\r\n|`[\"qwertyuiop\",\"asdfghjkl\",\"^zxcvbnm \"]`|`\"abc\"`|8",
        "_id": ObjectId("000000000000000000000002"),
        "isActive": true,
        "title": "Typing Distance",
        "testCases": [
            {
                "expectedOutput": "5",
                "input": "[\"qwertyuiop\",\"asdfghjkl\",\"^zxcvbnm \"]|\"qwerty\"",
                "isActive": true
            },
            {
                "expectedOutput": "7",
                "input": "[\"qwertyuiop\",\"asdfghjkl\",\"^zxcvbnm \"]|\"Qwerty\"",
                "isActive": true
            },
            {
                "expectedOutput": "8",
                "input": "[\"qwertyuiop\",\"asdfghjkl\",\"^zxcvbnm \"]|\"abc\"",
                "isActive": true
            },
        ]
    },
])