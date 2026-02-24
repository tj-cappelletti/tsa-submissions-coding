using System.Diagnostics.CodeAnalysis;
using Moq;
using Tsa.Submissions.Coding.UnitTests.Helpers.Languages;
using Tsa.Submissions.Coding.WebApi.Entities;
using Tsa.Submissions.Coding.WebApi.Services;

namespace Tsa.Submissions.Coding.UnitTests.Helpers.Mocks;

[ExcludeFromCodeCoverage]
internal class MockedProgrammingLanguagesServiceBuilder : MockedServiceBuilder<IProgrammingLanguagesService, ProgrammingLanguage>
{
    public override MockedServiceBuilder<IProgrammingLanguagesService, ProgrammingLanguage> WithCreateAsync(
        ProgrammingLanguage expectedEntity,
        string newId,
        Times? times = null)
    {
        WithCreateAsync(expectedEntity, newId, new ProgrammingLanguageEqualityComparer(), times);

        return this;
    }

    public override MockedServiceBuilder<IProgrammingLanguagesService, ProgrammingLanguage> WithRemoveAsync(
        ProgrammingLanguage entity,
        Times? times = null)
    {
        WithRemoveAsync(entity, new ProgrammingLanguageEqualityComparer(), times);

        return this;
    }

    public override MockedServiceBuilder<IProgrammingLanguagesService, ProgrammingLanguage> WithUpdateAsync(
        ProgrammingLanguage expectedEntity,
        Times? times = null)
    {
        WithUpdateAsync(expectedEntity, new ProgrammingLanguageEqualityComparer(), times);

        return this;
    }
}
