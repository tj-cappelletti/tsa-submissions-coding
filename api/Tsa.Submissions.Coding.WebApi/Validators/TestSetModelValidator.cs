using FluentValidation;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.WebApi.Validators;

public class TestSetModelValidator : AbstractValidator<TestSetModel>
{
    public TestSetModelValidator()
    {
        RuleFor(testSetModel => testSetModel.Name)
            .NotEmpty()
            .WithMessage("A name is required for the test set");

        RuleFor(testSetModel => testSetModel.Inputs)
            .SetValidator(new TestSetInputsPropertyValidator())
            .WithMessage("The input(s) are not valid");

        RuleFor(testSetModel => testSetModel.ProblemId)
            .NotEmpty()
            .WithMessage("A problem ID is required for the test set");
    }
}
