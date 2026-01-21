namespace Tsa.Submissions.Coding.Contracts;

public record BatchOperationResponse<TRequest, TResponse>
{
    public List<TResponse> CreatedItems { get; set; } = [];

    public List<TResponse> DeletedItems { get; set; } = [];

    public List<ItemFailureResponse<TRequest>> FailedItems { get; set; } = [];

    public string? Result { get; set; }

    public List<TResponse> UpdatedItems { get; set; } = [];
}
