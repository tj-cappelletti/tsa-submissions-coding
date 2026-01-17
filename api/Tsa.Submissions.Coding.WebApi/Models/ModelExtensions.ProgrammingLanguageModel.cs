using Tsa.Submissions.Coding.WebApi.Entities;

namespace Tsa.Submissions.Coding.WebApi.Models;

public partial class ModelExtensions
{
    public static ProgrammingLanguage ToEntity(this ProgrammingLanguageModel programmingLanguageModel)
    {
        return new ProgrammingLanguage
        {
            Name = programmingLanguageModel.Name,
            Version = programmingLanguageModel.Version
        };
    }
}
