using FluentValidation;
using Tsa.Submissions.Coding.Contracts.Languages;

namespace Tsa.Submissions.Coding.WebApi.Validators;

public class ProgrammingLanguageRequestValidator : AbstractValidator<ProgrammingLanguageRequest>
{
    public ProgrammingLanguageRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Identifier)
            .NotEmpty()
            .WithMessage("Identifier is required.")
            .Matches("^[a-z0-9_-]+$")
            .WithMessage("Identifier must contain only lowercase letters, numbers, hyphens, and underscores.")
            .MaximumLength(50)
            .WithMessage("Identifier must not exceed 50 characters.");

        RuleFor(x => x.FileExtension)
            .NotEmpty()
            .WithMessage("File extension is required.")
            .Matches(@"^\.[a-zA-Z0-9]+$")
            .WithMessage("File extension must start with a dot and contain only alphanumeric characters.")
            .MaximumLength(10)
            .WithMessage("File extension must not exceed 10 characters.");
    }
}
