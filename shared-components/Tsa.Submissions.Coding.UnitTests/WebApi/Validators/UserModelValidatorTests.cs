using System.Diagnostics.CodeAnalysis;
using FluentValidation.TestHelper;
using Tsa.Submissions.Coding.UnitTests.Data;
using Tsa.Submissions.Coding.WebApi.Models;
using Tsa.Submissions.Coding.WebApi.Validators;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Validators;

[ExcludeFromCodeCoverage]
public class UserModelValidatorTests
{
    [Theory]
    [ClassData(typeof(UserModelsTestData))]
    [Trait("TestCategory", "UnitTest")]
    public void Should_Successfully_Validate_ProblemModel(UserModel userModel, UserModelDataIssues userModelDataIssues)
    {
        // Arrange
        var userModelValidator = new UserModelValidator();

        // Act
        var testValidationResult = userModelValidator.TestValidate(userModel);

        // Assert
        if (userModelDataIssues == UserModelDataIssues.RoleNotValid)
        {
            testValidationResult.ShouldHaveValidationErrorFor(user => user.Role);
        }
        else
        {
            testValidationResult.ShouldNotHaveValidationErrorFor(user => user.Role);
        }

        if (userModelDataIssues == UserModelDataIssues.ParticipantMissingTeam)
        {
            testValidationResult.ShouldHaveValidationErrorFor(user => user.Team);
        }
        else
        {
            testValidationResult.ShouldNotHaveValidationErrorFor(user => user.Team);
        }

        if (userModelDataIssues == UserModelDataIssues.UserNameNotValid)
        {
            testValidationResult.ShouldHaveValidationErrorFor(user => user.UserName);
        }
        else
        {
            testValidationResult.ShouldNotHaveValidationErrorFor(user => user.UserName);
        }
    }
}
