using System.Collections.Generic;

namespace Tsa.Submissions.Coding.WebApi.Models;

public class BatchOperationModel<T>
{
    public List<T> CreatedItems { get; set; } = [];

    public List<T> DeletedItems { get; set; } = [];

    public List<ItemFailureModel<T>> FailedItems { get; set; } = [];

    public string? Result { get; set; }

    public List<T> UpdatedItems { get; set; } = [];
}

public enum BatchOperationResult
{
    Success,
    PartialSuccess,
    Failed
}
