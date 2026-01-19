using System.Diagnostics.CodeAnalysis;
using FluentValidation.TestHelper;
using Tsa.Submissions.Coding.UnitTests.Data;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Validators;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Validators;

[ExcludeFromCodeCoverage]
public class ProblemModelValidatorTests
{
    [Theory]
    [ClassData(typeof(ProblemsTestData))]
    [Trait("TestCategory", "UnitTest")]
    public void Should_Successfully_Validate_ProblemModel(Problem problem, ProblemDataIssues problemDataIssues)
    {
        // Arrange
        var problemModel = problem.ToModel();

        var problemModelValidator = new ProblemModelValidator();

        // Act

        var testValidationResult = problemModelValidator.TestValidate(problemModel);

        // Assert

        if (problemDataIssues.HasFlag(ProblemDataIssues.MissingDescription))
        {
            testValidationResult.ShouldHaveValidationErrorFor(_ => _.Description);
        }
        else
        {
            testValidationResult.ShouldNotHaveValidationErrorFor(_ => _.Description);
        }

        if (problemDataIssues.HasFlag(ProblemDataIssues.MissingTitle))
        {
            testValidationResult.ShouldHaveValidationErrorFor(_ => _.Title);
        }
        else
        {
            testValidationResult.ShouldNotHaveValidationErrorFor(_ => _.Title);
        }
    }
}
