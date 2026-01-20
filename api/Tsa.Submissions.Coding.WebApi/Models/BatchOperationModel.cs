using System.Collections.Generic;

namespace Tsa.Submissions.Coding.WebApi.Models;

public class BatchOperationModel<TRequest, TResponse>
{
    public List<TResponse> CreatedItems { get; set; } = [];

    public List<TResponse> DeletedItems { get; set; } = [];

    public List<ItemFailureModel<TRequest>> FailedItems { get; set; } = [];

    public string? Result { get; set; }

    public List<TResponse> UpdatedItems { get; set; } = [];
}

public enum BatchOperationResult
{
    Success,
    PartialSuccess,
    Failed
}
