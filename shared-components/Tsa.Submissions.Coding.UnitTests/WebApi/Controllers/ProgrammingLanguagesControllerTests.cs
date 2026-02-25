using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Tsa.Submissions.Coding.Contracts;
using Tsa.Submissions.Coding.Contracts.Languages;
using Tsa.Submissions.Coding.UnitTests.Data;
using Tsa.Submissions.Coding.UnitTests.Helpers.Languages;
using Tsa.Submissions.Coding.UnitTests.Helpers.Mocks;
using Tsa.Submissions.Coding.UnitTests.Helpers.Reflection;
using Tsa.Submissions.Coding.WebApi.Controllers;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Exceptions;
using Tsa.Submissions.Coding.WebApi.Services;
using Xunit;

namespace Tsa.Submissions.Coding.UnitTests.WebApi.Controllers;

[ExcludeFromCodeCoverage]
public class ProgrammingLanguagesControllerTests : ControllerTestsBase
{
    private static readonly Type ControllerType = typeof(ProgrammingLanguagesController);

    protected override string[] AllRolesMethods =>
    [
        "Get",
        "GetVersions"
    ];

    /// <summary>
    /// Creates a new instance of <see cref="ProgrammingLanguagesController"/> with the specified mocked dependencies.
    /// </summary>
    /// <param name="mockedService">The mocked programming languages service.</param>
    /// <param name="mockedProgrammingLanguageValidator">The mocked programming language request validator.</param>
    /// <param name="mockedProgrammingLanguageVersionValidator">The mocked programming language version request validator.</param>
    /// <returns>A new controller instance configured with the provided mocks.</returns>
    private static ProgrammingLanguagesController CreateController(
        IMock<IProgrammingLanguagesService> mockedService,
        IMock<IValidator<ProgrammingLanguageRequest>> mockedProgrammingLanguageValidator,
        IMock<IValidator<ProgrammingLanguageVersionRequest>> mockedProgrammingLanguageVersionValidator)
    {
        return new ProgrammingLanguagesController(
            mockedService.Object,
            mockedProgrammingLanguageValidator.Object,
            mockedProgrammingLanguageVersionValidator.Object);
    }

    /// <summary>
    /// Gets a valid <see cref="ProgrammingLanguage"/> instance from test data with no data issues.
    /// </summary>
    /// <returns>A valid programming language for testing.</returns>
    private static ProgrammingLanguage GetValidProgrammingLanguage()
    {
        var testDataSet = new ProgrammingLanguagesTestData();

        return (ProgrammingLanguage)testDataSet
            .First(data => (ProgrammingLanguageDataIssues)data[1] == ProgrammingLanguageDataIssues.None)[0];
    }

    /// <summary>
    /// Gets a list of valid <see cref="ProgrammingLanguage"/> instances from test data with no data issues.
    /// </summary>
    /// <returns>A list of valid programming languages for testing.</returns>
    private static List<ProgrammingLanguage> GetValidProgrammingLanguages()
    {
        var testDataSet = new ProgrammingLanguagesTestData();

        return testDataSet
            .Where(testData => (ProgrammingLanguageDataIssues)testData[1] == ProgrammingLanguageDataIssues.None)
            .Select(testData => (ProgrammingLanguage)testData[0])
            .ToList();
    }

    protected override string[] JudgeOnlyMethods =>
    [
        "Delete",
        "DeleteProgrammingLanguageVersion",
        "Post",
        "PostProgrammingLanguageVersion",
        "Put",
        "PutProgrammingLanguageVersion"
    ];

    /// <summary>
    ///     Verifies that all public controller methods have the
    ///     <see cref="Microsoft.AspNetCore.Authorization.AuthorizeAttribute" />
    ///     with the appropriate roles assigned.
    /// </summary>
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Public_Methods_Should_Have_Authorize_Attribute_With_Proper_Roles()
    {
        PublicMethodsHaveAuthorizeAttributeWithProperRoles(ControllerType);
    }

    /// <summary>
    ///     Verifies that all public controller methods have the appropriate HTTP method attribute.
    /// </summary>
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Public_Methods_Should_Have_Http_Method_Attribute()
    {
        PublicMethodsHaveHttpMethodAttribute(ControllerType);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Should_Have_ApiController_Attribute()
    {
        // Move this to the base class if this logic grows more complex
        // Single line assertion doesn't justify the need for a separate method at this time
        Assert.True(TypeHelpers.ClassHasSingleAttribute<ApiControllerAttribute>(ControllerType));
    }

    /// <summary>
    ///     Verifies that the expected number of public methods exists.
    ///     This test catches when methods are added or removed.
    /// </summary>
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Should_Have_Expected_Number_Of_Public_Methods()
    {
        ClassHasExpectedNumberOfPublicMethods(ControllerType);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Should_Have_Route_Attribute()
    {
        HasRouteAttribute(ControllerType, "api/programming-languages");
    }

    /// <summary>
    ///     Verifies that there are no public methods without HTTP method attributes.
    /// </summary>
    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public void Controller_Should_Not_Have_Public_Methods_Without_Http_Attributes()
    {
        PublicMethodsHaveHttpMethodAttribute(ControllerType);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Delete_Should_Return_No_Content()
    {
        // Arrange
        const string id = "64639f6fcdde06187b09ecae";

        var programmingLanguage = new ProgrammingLanguage
        {
            FileExtension = "test",
            Id = id,
            Identifier = "test",
            IsEnabled = true,
            Name = "Test"
        };

        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder()
            .WithGetAsync(id, programmingLanguage)
            .Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator().Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator().Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var result = await controller.Delete(id);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Delete_Should_Return_Not_Found()
    {
        // Arrange
        const string id = "64639f6fcdde06187b09ecae";

        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder()
            .WithGetAsync(id, null)
            .Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator().Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator().Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var result = await controller.Delete(id);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Delete_Versions_By_Id_And_Version_Tag_Should_Return_No_Content()
    {
        // Arrange
        var programmingLanguage = GetValidProgrammingLanguage();

        var id = programmingLanguage.Id;
        var version = programmingLanguage.Versions.First();
        var versionTag = version.VersionTag;

        var expectedProgrammingLanguage = new ProgrammingLanguage
        {
            FileExtension = programmingLanguage.FileExtension,
            Id = programmingLanguage.Id,
            Identifier = programmingLanguage.Identifier,
            IsEnabled = programmingLanguage.IsEnabled,
            Name = programmingLanguage.Name
        };

        foreach (var programmingLanguageVersion in programmingLanguage.Versions.Where(languageVersion => languageVersion.VersionTag != versionTag))
        {
            expectedProgrammingLanguage.Versions.Add(programmingLanguageVersion);
        }

        // We use the null forgiveness operator throughout this method because we assume the test data is setup correctly

        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder()
            .WithGetAsync(id!, programmingLanguage)
            .WithUpdateAsync(expectedProgrammingLanguage)
            .Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator().Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator().Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var actionResult = await controller.DeleteProgrammingLanguageVersion(id!, versionTag!);

        // Assert
        Assert.IsType<NoContentResult>(actionResult);

        mockedProgrammingLanguagesServices.VerifyAll();
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Delete_Versions_By_Id_And_Version_Tag_Should_Return_Not_Found_For_Programming_Language()
    {
        // Arrange
        const string id = "64639f6fcdde06187b09ecae";
        const string versionTag = "v1.0";

        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder()
            .WithGetAsync(id, null)
            .Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator().Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator().Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var actionResult = await controller.DeleteProgrammingLanguageVersion(id, versionTag);

        // Assert
        Assert.IsType<NotFoundResult>(actionResult);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Delete_Versions_By_Id_And_Version_Tag_Should_Return_Not_Found_For_Version_Tag()
    {
        // Arrange
        const string versionTag = "v1.0";

        var programmingLanguage = GetValidProgrammingLanguage();

        var id = programmingLanguage.Id;

        // We use the null forgiveness operator throughout this method because we assume the test data is setup correctly

        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder()
            .WithGetAsync(id!, programmingLanguage)
            .Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator().Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator().Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var actionResult = await controller.DeleteProgrammingLanguageVersion(id!, versionTag);

        // Assert
        Assert.IsType<NotFoundResult>(actionResult);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_By_Id_Should_Return_Not_Found()
    {
        // Arrange
        const string id = "64639f6fcdde06187b09ecae";

        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder()
            .WithGetAsync(id, null)
            .Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator().Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator().Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var result = await controller.Get(id);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_By_Id_Should_Return_Ok()
    {
        // Arrange
        var programmingLanguage = GetValidProgrammingLanguage();

        var id = programmingLanguage.Id;

        // We use the null forgiveness operator throughout this method because we assume the test data is setup correctly

        var expectedProgrammingLanguageVersionsResponse = programmingLanguage.Versions
            .Select(programmingLanguageVersion => new ProgrammingLanguageVersionResponse(
                programmingLanguageVersion.DisplayName!,
                programmingLanguageVersion.IsDefault,
                programmingLanguageVersion.VersionTag!))
            .ToList();

        var expectedProgrammingLanguageResponse = new ProgrammingLanguageResponse(
            programmingLanguage.Id!,
            programmingLanguage.Identifier!,
            programmingLanguage.Name!,
            programmingLanguage.FileExtension!,
            programmingLanguage.IsEnabled,
            expectedProgrammingLanguageVersionsResponse);

        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder()
            .WithGetAsync(id!, programmingLanguage)
            .Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator().Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator().Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var result = await controller.Get(id!);

        // Assert
        Assert.IsType<OkObjectResult>(result);

        var okObjectResult = (OkObjectResult)result;
        Assert.IsType<ProgrammingLanguageResponse>(okObjectResult.Value);

        var actualProgrammingLanguageResponse = (ProgrammingLanguageResponse)okObjectResult.Value;
        Assert.Equal(expectedProgrammingLanguageResponse, actualProgrammingLanguageResponse, new ProgrammingLanguageResponseEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_Should_Return_Ok()
    {
        // Arrange
        var programmingLanguages = GetValidProgrammingLanguages();

        // We use the null forgiveness operator throughout this method because we assume the test data is setup correctly

        var expectedProgrammingLanguageResponses = new List<ProgrammingLanguageResponse>();

        foreach (var programmingLanguage in programmingLanguages)
        {
            var expectedProgrammingLanguageVersionsResponse = programmingLanguage.Versions
                .Select(programmingLanguageVersion => new ProgrammingLanguageVersionResponse(
                    programmingLanguageVersion.DisplayName!,
                    programmingLanguageVersion.IsDefault,
                    programmingLanguageVersion.VersionTag!))
                .ToList();

            var expectedProgrammingLanguageResponse = new ProgrammingLanguageResponse(
                programmingLanguage.Id!,
                programmingLanguage.Identifier!,
                programmingLanguage.Name!,
                programmingLanguage.FileExtension!,
                programmingLanguage.IsEnabled,
                expectedProgrammingLanguageVersionsResponse);

            expectedProgrammingLanguageResponses.Add(expectedProgrammingLanguageResponse);
        }

        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder()
            .WithGetAsync(programmingLanguages)
            .Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator().Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator().Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var programmingLanguageResponses = await controller.Get();

        // Assert
        Assert.NotEmpty(programmingLanguageResponses);

        Assert.Equal(expectedProgrammingLanguageResponses, programmingLanguageResponses, new ProgrammingLanguageResponseEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_Should_Return_Ok_When_Empty()
    {
        // Arrange
        var programmingLanguages = new List<ProgrammingLanguage>();

        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder()
            .WithGetAsync(programmingLanguages)
            .Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator().Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator().Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var programmingLanguageResponses = await controller.Get();

        // Assert
        Assert.Empty(programmingLanguageResponses);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_Versions_By_Id_Should_Return_Not_Found_For_Programming_Language()
    {
        // Arrange
        const string id = "64639f6fcdde06187b09ecae";

        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder()
            .WithGetAsync(id, null)
            .Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator().Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator().Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var actionResult = await controller.GetVersions(id);

        // Assert
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_Versions_By_Id_Should_Return_Ok()
    {
        // Arrange
        var programmingLanguage = GetValidProgrammingLanguage();

        var id = programmingLanguage.Id;

        // We use the null forgiveness operator throughout this method because we assume the test data is setup correctly

        var expectedProgrammingLanguageVersionsResponse = programmingLanguage.Versions
            .Select(programmingLanguageVersion => new ProgrammingLanguageVersionResponse(
                programmingLanguageVersion.DisplayName!,
                programmingLanguageVersion.IsDefault,
                programmingLanguageVersion.VersionTag!))
            .ToList();

        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder()
            .WithGetAsync(id!, programmingLanguage)
            .Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator().Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator().Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var actionResult = await controller.GetVersions(id!);

        // Assert
        Assert.IsType<OkObjectResult>(actionResult.Result);

        var okObjectResult = (OkObjectResult)actionResult.Result;
        Assert.IsType<List<ProgrammingLanguageVersionResponse>>(okObjectResult.Value);

        var actualProgrammingLanguageVersionResponses = (List<ProgrammingLanguageVersionResponse>)okObjectResult.Value;
        Assert.Equal(expectedProgrammingLanguageVersionsResponse, actualProgrammingLanguageVersionResponses,
            new ProgrammingLanguageVersionResponseEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Get_Versions_By_Id_Should_Return_Ok_When_Empty()
    {
        // Arrange
        var programmingLanguage = GetValidProgrammingLanguage();

        var id = programmingLanguage.Id;

        programmingLanguage.Versions.Clear();

        // We use the null forgiveness operator throughout this method because we assume the test data is setup correctly

        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder()
            .WithGetAsync(id!, programmingLanguage)
            .Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator().Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator().Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var actionResult = await controller.GetVersions(id!);

        // Assert
        Assert.IsType<OkObjectResult>(actionResult.Result);

        var okObjectResult = (OkObjectResult)actionResult.Result;
        Assert.IsType<List<ProgrammingLanguageVersionResponse>>(okObjectResult.Value);

        var actualProgrammingLanguageVersionResponses = (List<ProgrammingLanguageVersionResponse>)okObjectResult.Value;
        Assert.Empty(actualProgrammingLanguageVersionResponses);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Post_Should_Return_Bad_Request()
    {
        // Arrange
        var programmingLanguageRequest = new ProgrammingLanguageRequest("TEST", "Test", ".test", true);

        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder().Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator()
            .WithFailedValidationResult(programmingLanguageRequest, "FileExtension", "File extension is invalid")
            .Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator().Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var actionResult = await controller.Post(programmingLanguageRequest);

        // Assert
        Assert.IsType<BadRequestObjectResult>(actionResult);

        var badRequestObjectResult = (BadRequestObjectResult)actionResult;
        Assert.IsType<ValidationProblemDetails>(badRequestObjectResult.Value);

        var validationProblemDetails = (ValidationProblemDetails)badRequestObjectResult.Value;
        Assert.True(validationProblemDetails.Errors.ContainsKey("FileExtension"));
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Post_Should_Return_Conflict()
    {
        // Arrange
        var programmingLanguage = GetValidProgrammingLanguage();

        const int expectedErrorCode = (int)ErrorCodes.EntityAlreadyExists;
        const string expectedErrorMessage = "The resource requested to create already exists.";
        const string expectedEntity = nameof(ProgrammingLanguage);
        
        var expectedLookupKey = programmingLanguage.Name;

        var programmingLanguageRequest = new ProgrammingLanguageRequest(
            programmingLanguage.Identifier!,
            programmingLanguage.Name!,
            programmingLanguage.FileExtension!,
            programmingLanguage.IsEnabled);

        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder()
            .WithGetAsync([programmingLanguage])
            .Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator()
            .WithSuccessfulValidationResult(programmingLanguageRequest)
            .Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator().Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var actionResult = await controller.Post(programmingLanguageRequest);

        // Assert
        Assert.IsType<ConflictObjectResult>(actionResult);

        var conflictObjectResult = (ConflictObjectResult)actionResult;
        Assert.IsType<ApiErrorResponse>(conflictObjectResult.Value);

        var apiErrorResponse = (ApiErrorResponse)conflictObjectResult.Value;
        Assert.Equal(expectedLookupKey, apiErrorResponse.Data["lookupKey"]);
        Assert.Equal(expectedEntity, apiErrorResponse.Data["entityName"]);
        Assert.Equal(expectedErrorCode, apiErrorResponse.ErrorCode);
        Assert.Equal(expectedErrorMessage, apiErrorResponse.Message);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Post_Should_Return_Conflict_When_Name_Already_Exists()
    {
        // Arrange
        var programmingLanguage = GetValidProgrammingLanguage();

        const int expectedErrorCode = (int)ErrorCodes.EntityAlreadyExists;
        const string expectedErrorMessage = "The resource requested to create already exists.";
        const string expectedEntity = nameof(ProgrammingLanguage);

        var expectedLookupKey = programmingLanguage.Name;

        // Use same Identifier but different Name to trigger the name conflict check
        var programmingLanguageRequest = new ProgrammingLanguageRequest(
            "DIFFERENT_IDENTIFIER",
            programmingLanguage.Name!,
            programmingLanguage.FileExtension!,
            programmingLanguage.IsEnabled);

        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder()
            .WithGetAsync([programmingLanguage])
            .Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator()
            .WithSuccessfulValidationResult(programmingLanguageRequest)
            .Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator().Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var actionResult = await controller.Post(programmingLanguageRequest);

        // Assert
        Assert.IsType<ConflictObjectResult>(actionResult);

        var conflictObjectResult = (ConflictObjectResult)actionResult;
        Assert.IsType<ApiErrorResponse>(conflictObjectResult.Value);

        var apiErrorResponse = (ApiErrorResponse)conflictObjectResult.Value;
        Assert.Equal(expectedLookupKey, apiErrorResponse.Data["lookupKey"]);
        Assert.Equal(expectedEntity, apiErrorResponse.Data["entityName"]);
        Assert.Equal(expectedErrorCode, apiErrorResponse.ErrorCode);
        Assert.Equal(expectedErrorMessage, apiErrorResponse.Message);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Post_Should_Return_Conflict_When_Identifier_Already_Exists()
    {
        // Arrange
        var programmingLanguage = GetValidProgrammingLanguage();

        const int expectedErrorCode = (int)ErrorCodes.EntityAlreadyExists;
        const string expectedErrorMessage = "The resource requested to create already exists.";
        const string expectedEntity = nameof(ProgrammingLanguage);

        var expectedLookupKey = programmingLanguage.Identifier;

        // Use same Identifier but different Name to trigger the identifier conflict check
        var programmingLanguageRequest = new ProgrammingLanguageRequest(
            programmingLanguage.Identifier!,
            "A Different Name",
            programmingLanguage.FileExtension!,
            programmingLanguage.IsEnabled);

        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder()
            .WithGetAsync([programmingLanguage])
            .Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator()
            .WithSuccessfulValidationResult(programmingLanguageRequest)
            .Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator().Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var actionResult = await controller.Post(programmingLanguageRequest);

        // Assert
        Assert.IsType<ConflictObjectResult>(actionResult);

        var conflictObjectResult = (ConflictObjectResult)actionResult;
        Assert.IsType<ApiErrorResponse>(conflictObjectResult.Value);

        var apiErrorResponse = (ApiErrorResponse)conflictObjectResult.Value;
        Assert.Equal(expectedLookupKey, apiErrorResponse.Data["lookupKey"]);
        Assert.Equal(expectedEntity, apiErrorResponse.Data["entityName"]);
        Assert.Equal(expectedErrorCode, apiErrorResponse.ErrorCode);
        Assert.Equal(expectedErrorMessage, apiErrorResponse.Message);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Post_Should_Return_Created()
    {
        // Arrange
        const string id = "64639f6fcdde06187b09ecae";

        var programmingLanguageRequest = new ProgrammingLanguageRequest("TEST", "Test", ".test", true);

        var expectedProgrammingLanguage = new ProgrammingLanguage
        {
            FileExtension = programmingLanguageRequest.FileExtension,
            Identifier = programmingLanguageRequest.Identifier,
            IsEnabled = programmingLanguageRequest.IsEnabled,
            Name = programmingLanguageRequest.Name
        };

        var expectedProgrammingLanguageResponse = new ProgrammingLanguageResponse(
            id,
            programmingLanguageRequest.Identifier,
            programmingLanguageRequest.Name,
            programmingLanguageRequest.FileExtension,
            programmingLanguageRequest.IsEnabled,
            new List<ProgrammingLanguageVersionResponse>());

        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder()
            .WithGetAsync([])
            .WithCreateAsync(expectedProgrammingLanguage, id)
            .Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator()
            .WithSuccessfulValidationResult(programmingLanguageRequest)
            .Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator().Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var actionResult = await controller.Post(programmingLanguageRequest);

        // Assert
        Assert.IsType<CreatedAtActionResult>(actionResult);

        var createdAtActionResult = (CreatedAtActionResult)actionResult;
        Assert.Equal("Get", createdAtActionResult.ActionName);
        Assert.IsType<ProgrammingLanguageResponse>(createdAtActionResult.Value);

        var actualProgrammingLanguageResponse = (ProgrammingLanguageResponse)createdAtActionResult.Value;
        Assert.Equal(expectedProgrammingLanguageResponse, actualProgrammingLanguageResponse, new ProgrammingLanguageResponseEqualityComparer());
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task PostProgrammingLanguageVersion_Should_Return_Bad_Request()
    {
        // Arrange
        var programmingLanguage = GetValidProgrammingLanguage();

        var id = programmingLanguage.Id!;

        var programmingLanguageVersionRequest = new ProgrammingLanguageVersionRequest("No DisplayName", true, "1.0.0");

        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder()
            .WithGetAsync(id, programmingLanguage)
            .Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator().Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator()
            .WithFailedValidationResult(programmingLanguageVersionRequest, "DisplayName", "Display name is required")
            .Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var actionResult = await controller.PostProgrammingLanguageVersion(id, programmingLanguageVersionRequest);

        // Assert
        Assert.IsType<BadRequestObjectResult>(actionResult);

        var badRequestObjectResult = (BadRequestObjectResult)actionResult;
        Assert.IsType<ValidationProblemDetails>(badRequestObjectResult.Value);

        var validationProblemDetails = (ValidationProblemDetails)badRequestObjectResult.Value;
        Assert.True(validationProblemDetails.Errors.ContainsKey("DisplayName"));
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task PostProgrammingLanguageVersion_Should_Return_Conflict()
    {
        // Arrange
        var programmingLanguage = GetValidProgrammingLanguage();

        var id = programmingLanguage.Id!;

        var programmingLanguageVersion = programmingLanguage.Versions.First();

        var programmingLanguageVersionRequest = new ProgrammingLanguageVersionRequest(
            programmingLanguageVersion.DisplayName!,
            programmingLanguageVersion.IsDefault,
            programmingLanguageVersion.VersionTag!);

        var expectedLookupKey = $"{programmingLanguageVersionRequest.DisplayName}.{programmingLanguageVersionRequest.VersionTag}";
        const string expectedEntity = nameof(ProgrammingLanguageVersion);
        const int expectedErrorCode = (int)ErrorCodes.EntityAlreadyExists;
        const string expectedErrorMessage = "The resource requested to create already exists.";

        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder()
            .WithGetAsync(id, programmingLanguage)
            .Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator().Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator()
            .WithSuccessfulValidationResult(programmingLanguageVersionRequest)
            .Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var result = await controller.PostProgrammingLanguageVersion(id, programmingLanguageVersionRequest);

        // Assert
        Assert.IsType<ConflictObjectResult>(result);

        var conflictObjectResult = (ConflictObjectResult)result;
        Assert.IsType<ApiErrorResponse>(conflictObjectResult.Value);

        var apiErrorResponse = (ApiErrorResponse)conflictObjectResult.Value;
        Assert.Equal(expectedLookupKey, apiErrorResponse.Data["lookupKey"]);
        Assert.Equal(expectedEntity, apiErrorResponse.Data["entityName"]);
        Assert.Equal(expectedErrorCode, apiErrorResponse.ErrorCode);
        Assert.Equal(expectedErrorMessage, apiErrorResponse.Message);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task PostProgrammingLanguageVersion_Should_Return_Created()
    {
        // Arrange
        var programmingLanguage = GetValidProgrammingLanguage();

        var id = programmingLanguage.Id!;

        var programmingLanguageVersionRequest = new ProgrammingLanguageVersionRequest("Test", true, "test");

        var expectedProgrammingLanguage = new ProgrammingLanguage
        {
            FileExtension = programmingLanguage.FileExtension,
            Id = programmingLanguage.Id,
            Identifier = programmingLanguage.Identifier,
            IsEnabled = programmingLanguage.IsEnabled,
            Name = programmingLanguage.Name
        };

        foreach (var programmingLanguageVersion in programmingLanguage.Versions)
        {
            expectedProgrammingLanguage.Versions.Add(new ProgrammingLanguageVersion
            {
                DisplayName = programmingLanguageVersion.DisplayName,
                IsDefault = programmingLanguageVersion.IsDefault,
                VersionTag = programmingLanguageVersion.VersionTag
            });
        }

        expectedProgrammingLanguage.Versions.Add(new ProgrammingLanguageVersion
        {
            DisplayName = programmingLanguageVersionRequest.DisplayName,
            IsDefault = programmingLanguageVersionRequest.IsDefault,
            VersionTag = programmingLanguageVersionRequest.VersionTag
        });

        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder()
            .WithGetAsync(id, programmingLanguage)
            .Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator().Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator()
            .WithSuccessfulValidationResult(programmingLanguageVersionRequest)
            .Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var result = await controller.PostProgrammingLanguageVersion(id, programmingLanguageVersionRequest);

        // Assert
        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task PostProgrammingLanguageVersion_Should_Return_Not_Found_For_Programming_Language()
    {
        // Arrange
        const string id = "64639f6fcdde06187b09ecae";

        var programmingLanguageVersionRequest = new ProgrammingLanguageVersionRequest("Test", true, "test");

        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder()
            .WithGetAsync(id, null)
            .Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator().Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator().Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var result = await controller.PostProgrammingLanguageVersion(id, programmingLanguageVersionRequest);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Put_Should_Return_Bad_Request()
    {
        // Arrange
        var programmingLanguage = GetValidProgrammingLanguage();

        var id = programmingLanguage.Id!;

        var programmingLanguageRequest = new ProgrammingLanguageRequest("TEST", "Test", ".test", true);

        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder().Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator()
            .WithFailedValidationResult(programmingLanguageRequest, "FileExtension", "File extension is invalid")
            .Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator().Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var actionResult = await controller.Put(id, programmingLanguageRequest);

        // Assert
        Assert.IsType<BadRequestObjectResult>(actionResult);

        var badRequestObjectResult = (BadRequestObjectResult)actionResult;
        Assert.IsType<ValidationProblemDetails>(badRequestObjectResult.Value);

        var validationProblemDetails = (ValidationProblemDetails)badRequestObjectResult.Value;
        Assert.True(validationProblemDetails.Errors.ContainsKey("FileExtension"));
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Put_Should_Return_Conflict_When_Name_Already_Exists()
    {
        // Arrange
        var programmingLanguages = GetValidProgrammingLanguages();

        var existingProgrammingLanguage = programmingLanguages[0];

        var programmingLanguage = programmingLanguages[1];

        var id = programmingLanguage.Id;

        const int expectedErrorCode = (int)ErrorCodes.EntityAlreadyExists;
        const string expectedErrorMessage = "The resource requested to create already exists.";
        const string expectedEntity = nameof(ProgrammingLanguage);

        var expectedLookupKey = existingProgrammingLanguage.Name;

        var programmingLanguageRequest = new ProgrammingLanguageRequest(
            programmingLanguage.Identifier!,
            // Use the name of the existing programming language to trigger the conflict
            existingProgrammingLanguage.Name!,
            programmingLanguage.FileExtension!,
            programmingLanguage.IsEnabled);

        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder()
            .WithGetAsync(id!, programmingLanguage)
            .WithGetAsync(programmingLanguages)
            .Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator()
            .WithSuccessfulValidationResult(programmingLanguageRequest)
            .Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator().Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var actionResult = await controller.Put(id!, programmingLanguageRequest);

        // Assert
        Assert.IsType<ConflictObjectResult>(actionResult);

        var conflictObjectResult = (ConflictObjectResult)actionResult;
        Assert.IsType<ApiErrorResponse>(conflictObjectResult.Value);

        var apiErrorResponse = (ApiErrorResponse)conflictObjectResult.Value;
        Assert.Equal(expectedLookupKey, apiErrorResponse.Data["lookupKey"]);
        Assert.Equal(expectedEntity, apiErrorResponse.Data["entityName"]);
        Assert.Equal(expectedErrorCode, apiErrorResponse.ErrorCode);
        Assert.Equal(expectedErrorMessage, apiErrorResponse.Message);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Put_Should_Return_Conflict_When_Identifier_Already_Exists()
    {
        // Arrange
        var programmingLanguages = GetValidProgrammingLanguages();

        var existingProgrammingLanguage = programmingLanguages[0];

        var programmingLanguage = programmingLanguages[1];

        var id = programmingLanguage.Id;

        const int expectedErrorCode = (int)ErrorCodes.EntityAlreadyExists;
        const string expectedErrorMessage = "The resource requested to create already exists.";
        const string expectedEntity = nameof(ProgrammingLanguage);

        var expectedLookupKey = existingProgrammingLanguage.Identifier;

        var programmingLanguageRequest = new ProgrammingLanguageRequest(
            // Use the identifier of the existing programming language to trigger the conflict
            existingProgrammingLanguage.Identifier!,
            programmingLanguage.Name!,
            programmingLanguage.FileExtension!,
            programmingLanguage.IsEnabled);

        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder()
            .WithGetAsync(id!, programmingLanguage)
            .WithGetAsync(programmingLanguages)
            .Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator()
            .WithSuccessfulValidationResult(programmingLanguageRequest)
            .Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator().Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var actionResult = await controller.Put(id!, programmingLanguageRequest);

        // Assert
        Assert.IsType<ConflictObjectResult>(actionResult);

        var conflictObjectResult = (ConflictObjectResult)actionResult;
        Assert.IsType<ApiErrorResponse>(conflictObjectResult.Value);

        var apiErrorResponse = (ApiErrorResponse)conflictObjectResult.Value;
        Assert.Equal(expectedLookupKey, apiErrorResponse.Data["lookupKey"]);
        Assert.Equal(expectedEntity, apiErrorResponse.Data["entityName"]);
        Assert.Equal(expectedErrorCode, apiErrorResponse.ErrorCode);
        Assert.Equal(expectedErrorMessage, apiErrorResponse.Message);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Put_Should_Return_No_Content()
    {
        // Arrange
        var programmingLanguages = GetValidProgrammingLanguages();

        var programmingLanguage = programmingLanguages[0];

        var id = programmingLanguage.Id;

        var programmingLanguageRequest = new ProgrammingLanguageRequest("TEST", "Test", ".test", true);

        var expectedProgrammingLanguage = new ProgrammingLanguage
        {
            FileExtension = programmingLanguageRequest.FileExtension,
            Id = programmingLanguage.Id,
            Identifier = programmingLanguageRequest.Identifier,
            IsEnabled = programmingLanguageRequest.IsEnabled,
            Name = programmingLanguageRequest.Name,
            Versions = programmingLanguage.Versions
        };

        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder()
            .WithGetAsync(id!, programmingLanguage)
            .WithGetAsync(programmingLanguages)
            .WithUpdateAsync(expectedProgrammingLanguage)
            .Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator()
            .WithSuccessfulValidationResult(programmingLanguageRequest)
            .Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator().Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var actionResult = await controller.Put(id!, programmingLanguageRequest);

        // Assert
        Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task Put_Should_Return_Not_Found()
    {
        // Arrange
        const string id = "64639f6fcdde06187b09ecae";

        var programmingLanguageRequest = new ProgrammingLanguageRequest("TEST", "Test", ".test", true);

        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder()
            .WithGetAsync(id, null)
            .Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator()
            .WithSuccessfulValidationResult(programmingLanguageRequest)
            .Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator().Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var actionResult = await controller.Put(id, programmingLanguageRequest);

        // Assert
        Assert.IsType<NotFoundResult>(actionResult);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task PutProgrammingLanguageVersion_Should_Return_Bad_Request()
    {
        // Arrange
        var programmingLanguage = GetValidProgrammingLanguage();

        var id = programmingLanguage.Id!;

        var programmingLanguageVersionRequest = new ProgrammingLanguageVersionRequest("No DisplayName", true, "1.0.0");

        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder()
            .WithGetAsync(id, programmingLanguage)
            .Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator().Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator()
            .WithFailedValidationResult(programmingLanguageVersionRequest, "DisplayName", "Display name is required")
            .Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var actionResult = await controller.PutProgrammingLanguageVersion(id, programmingLanguageVersionRequest);

        // Assert
        Assert.IsType<BadRequestObjectResult>(actionResult);

        var badRequestObjectResult = (BadRequestObjectResult)actionResult;
        Assert.IsType<ValidationProblemDetails>(badRequestObjectResult.Value);

        var validationProblemDetails = (ValidationProblemDetails)badRequestObjectResult.Value;
        Assert.True(validationProblemDetails.Errors.ContainsKey("DisplayName"));
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task PutProgrammingLanguageVersion_Should_Return_No_Content()
    {
        // Arrange
        var programmingLanguage = GetValidProgrammingLanguage();

        var id = programmingLanguage.Id;

        var programmingLanguageVersion = programmingLanguage.Versions.First();

        var programmingLanguageVersionRequest = new ProgrammingLanguageVersionRequest(
            "Test",
            programmingLanguageVersion.IsDefault,
            programmingLanguageVersion.VersionTag!);

        var expectedProgrammingLanguage = new ProgrammingLanguage
        {
            FileExtension = programmingLanguage.FileExtension,
            Id = programmingLanguage.Id,
            Identifier = programmingLanguage.Identifier,
            IsEnabled = programmingLanguage.IsEnabled,
            Name = programmingLanguage.Name
        };

        foreach (var languageVersion in programmingLanguage.Versions)
        {
            if (languageVersion.VersionTag == programmingLanguageVersionRequest.VersionTag)
            {
                expectedProgrammingLanguage.Versions.Add(new ProgrammingLanguageVersion
                {
                    DisplayName = programmingLanguageVersionRequest.DisplayName,
                    IsDefault = programmingLanguageVersionRequest.IsDefault,
                    VersionTag = programmingLanguageVersionRequest.VersionTag
                });
            }
            else
            {
                expectedProgrammingLanguage.Versions.Add(new ProgrammingLanguageVersion
                {
                    DisplayName = languageVersion.DisplayName,
                    IsDefault = languageVersion.IsDefault,
                    VersionTag = languageVersion.VersionTag
                });
            }
        }

        // We use the null forgiveness operator throughout this method because we assume the test data is setup correctly
        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder()
            .WithGetAsync(id!, programmingLanguage)
            .Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator().Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator()
            .WithSuccessfulValidationResult(programmingLanguageVersionRequest)
            .Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var result = await controller.PutProgrammingLanguageVersion(id!, programmingLanguageVersionRequest);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task PutProgrammingLanguageVersion_Should_Return_No_Content_When_IsDefault_Changes()
    {
        // Arrange
        var programmingLanguage = GetValidProgrammingLanguage();

        var id = programmingLanguage.Id;

        var programmingLanguageVersion = programmingLanguage.Versions.First(version => !version.IsDefault);

        var programmingLanguageVersionRequest = new ProgrammingLanguageVersionRequest(
            "Test",
            true,
            programmingLanguageVersion.VersionTag!);

        var expectedProgrammingLanguage = new ProgrammingLanguage
        {
            FileExtension = programmingLanguage.FileExtension,
            Id = programmingLanguage.Id,
            Identifier = programmingLanguage.Identifier,
            IsEnabled = programmingLanguage.IsEnabled,
            Name = programmingLanguage.Name
        };

        foreach (var languageVersion in programmingLanguage.Versions)
        {
            if (languageVersion.VersionTag == programmingLanguageVersionRequest.VersionTag)
            {
                expectedProgrammingLanguage.Versions.Add(new ProgrammingLanguageVersion
                {
                    DisplayName = programmingLanguageVersionRequest.DisplayName,
                    IsDefault = programmingLanguageVersionRequest.IsDefault,
                    VersionTag = programmingLanguageVersionRequest.VersionTag
                });
            }
            else
            {
                expectedProgrammingLanguage.Versions.Add(new ProgrammingLanguageVersion
                {
                    DisplayName = languageVersion.DisplayName,
                    IsDefault = false,
                    VersionTag = languageVersion.VersionTag
                });
            }
        }

        // We use the null forgiveness operator throughout this method because we assume the test data is setup correctly
        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder()
            .WithGetAsync(id!, programmingLanguage)
            .Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator().Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator()
            .WithSuccessfulValidationResult(programmingLanguageVersionRequest)
            .Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var result = await controller.PutProgrammingLanguageVersion(id!, programmingLanguageVersionRequest);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task PutProgrammingLanguageVersion_Should_Return_Not_Found_For_Programming_Language()
    {
        // Arrange
        const string id = "64639f6fcdde06187b09ecae";

        var programmingLanguageVersionRequest = new ProgrammingLanguageVersionRequest(
            "Test",
            false,
            "v1.0");

        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder()
            .WithGetAsync(id, null)
            .Build();

        // Validation of the request should occur after checking if the programming language exists, so the validator should not be called in this scenario
        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator().Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator().Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var result = await controller.PutProgrammingLanguageVersion(id, programmingLanguageVersionRequest);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    [Trait("TestCategory", "UnitTest")]
    public async Task PutProgrammingLanguageVersion_Should_Return_Not_Found_For_Version_Tag()
    {
        // Arrange
        var programmingLanguage = GetValidProgrammingLanguage();

        var id = programmingLanguage.Id;

        var programmingLanguageVersionRequest = new ProgrammingLanguageVersionRequest("Test", true, "TEST");

        var mockedProgrammingLanguagesServices = new MockedProgrammingLanguagesServiceBuilder()
            .WithGetAsync(id!, programmingLanguage)
            .Build();

        var mockedProgrammingLanguageRequestValidator = new MockedProgrammingLanguageRequestValidator().Build();

        var mockedProgrammingLanguageVersionRequestValidator = new MockedProgrammingLanguageVersionRequestValidator()
            .WithSuccessfulValidationResult(programmingLanguageVersionRequest)
            .Build();

        var controller = CreateController(
            mockedProgrammingLanguagesServices,
            mockedProgrammingLanguageRequestValidator,
            mockedProgrammingLanguageVersionRequestValidator
        );

        // Act
        var result = await controller.PutProgrammingLanguageVersion(id!, programmingLanguageVersionRequest);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
