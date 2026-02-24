using System.Diagnostics.CodeAnalysis;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Mocks;

[ExcludeFromCodeCoverage]
internal class MockedProgrammingLanguagesServiceBuilder : MockedServiceBuilder<IProgrammingLanguagesService, ProgrammingLanguage>;
