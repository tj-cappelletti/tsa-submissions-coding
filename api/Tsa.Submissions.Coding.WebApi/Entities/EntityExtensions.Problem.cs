using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public static partial class EntityExtensions
{
    private static List<ProblemModel> ProblemsToProblemModels(IEnumerable<Problem> problems)
    {
        return problems.Select(problem => problem.ToModel()).ToList();
    }

    public static ProblemModel ToModel(this Problem problem)
    {
        return new ProblemModel
        {
            Description = problem.Description,
            Id = problem.Id,
            IsActive = problem.IsActive,
            Title = problem.Title
        };
    }

    public static List<ProblemModel> ToModels(this IList<Problem> problems)
    {
        return ProblemsToProblemModels(problems);
    }

    public static List<ProblemModel> ToModels(this IEnumerable<Problem> problems)
    {
        return ProblemsToProblemModels(problems);
    }
}
