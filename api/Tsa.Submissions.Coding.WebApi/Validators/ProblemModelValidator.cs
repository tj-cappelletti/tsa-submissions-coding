using FluentValidation;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.WebApi.Validators;

public class ProblemModelValidator : AbstractValidator<ProblemModel>
{
    public ProblemModelValidator()
    {
        RuleFor(problem => problem.Description)
            .NotEmpty()
            .WithMessage("A problem must have a description.");

        RuleFor(problem => problem.Title)
            .NotEmpty()
            .WithMessage("A problem must have title.");
    }
}
