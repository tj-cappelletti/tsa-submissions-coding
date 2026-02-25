using FluentValidation;
using Tsa.Submissions.Coding.Contracts.Languages;

namespace Tsa.Submissions.Coding.WebApi.Validators;

public class ProgrammingLanguageVersionRequestValidator : AbstractValidator<ProgrammingLanguageVersionRequest>
{
    public ProgrammingLanguageVersionRequestValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .WithMessage("Display name is required.");

        RuleFor(x => x.VersionTag)
            .NotEmpty()
            .WithMessage("Version tag is required.");
    }
}
