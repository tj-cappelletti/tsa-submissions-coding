using FluentValidation;
using Tsa.Submissions.Coding.Contracts.Problems;

namespace Tsa.Submissions.Coding.WebApi.Validators;

public class ProblemRequestValidator : AbstractValidator<ProblemRequest>
{
    public ProblemRequestValidator()
    {
        RuleFor(problem => problem.Title)
            .NotEmpty()
            .WithMessage("The title of the problem is required.");

        RuleFor(problem => problem.Description)
            .NotEmpty()
            .WithMessage("The description of the problem is required.");
    }
}
