using FluentValidation;
using Tsa.Submissions.Coding.Contracts.Submissions;

namespace Tsa.Submissions.Coding.WebApi.Validators;

public class SubmissionCreateRequestValidator : AbstractValidator<SubmissionCreateRequest>
{
    public SubmissionCreateRequestValidator()
    {
        RuleFor(submission => submission.ProblemId)
            .NotEmpty()
            .WithMessage("The Problem ID is required for a submission.");

        RuleFor(submission => submission.ProgrammingLanguageId)
            .NotEmpty()
            .WithMessage("The Programming Language ID is required for a submission.");

        RuleFor(submission => submission.ProgrammingLanguageVersionTag)
            .NotEmpty()
            .WithMessage("The Programming Language Version Tag is required for a submission.");

        RuleFor(submission => submission.Solution)
            .NotEmpty()
            .WithMessage("A solution is required for a submission.");
    }
}
