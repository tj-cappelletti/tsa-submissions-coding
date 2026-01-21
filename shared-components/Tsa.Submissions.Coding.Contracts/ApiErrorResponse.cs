namespace Tsa.Submissions.Coding.Contracts;

public record ApiErrorResponse
{
    public Dictionary<string, string> Data { get; init; } = new();

    public int ErrorCode { get; init; }

    public string Message { get; init; } = string.Empty;

    public ApiErrorResponse(Dictionary<string, string> data, int errorCode, string message)
    {
        Data = data;
        ErrorCode = errorCode;
        Message = message;
    }

    public ApiErrorResponse(int errorCode, string message)
    {
        ErrorCode = errorCode;
        Message = message;
    }
}
