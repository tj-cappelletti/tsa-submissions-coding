using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Models;

public static partial class ModelExtensions
{
    public static Problem ToEntity(this ProblemModel problemModel)
    {
        return new Problem
        {
            Description = problemModel.Description,
            Id = problemModel.Id,
            IsActive = problemModel.IsActive,
            Title = problemModel.Title
        };
    }
}
