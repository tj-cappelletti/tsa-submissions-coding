namespace Tsa.Submissions.Coding.WebApi.Models;

public class ItemFailureModel<T>
{
    public string? ErrorMessage { get; set; }

    public T? Item { get; set; }
}
