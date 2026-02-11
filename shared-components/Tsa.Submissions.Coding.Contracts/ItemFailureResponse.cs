namespace Tsa.Submissions.Coding.Contracts;

public record ItemFailureResponse<T>(string ErrorMessage, T Item);
