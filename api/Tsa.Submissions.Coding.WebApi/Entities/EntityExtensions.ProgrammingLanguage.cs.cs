using System.Collections.Generic;
using System.Linq;
using Tsa.Submissions.Coding.WebApi.Models;

namespace Tsa.Submissions.Coding.WebApi.Entities;

public partial class EntityExtensions
{
    private static List<ProgrammingLanguageModel> ProgrammingLanguagesToModels(IEnumerable<ProgrammingLanguage> programmingLanguages)
    {
        return programmingLanguages.Select(selector: programmingLanguage => programmingLanguage.ToModel()).ToList();
    }

    public static ProgrammingLanguageModel ToModel(this ProgrammingLanguage programmingLanguage)
    {
        return new ProgrammingLanguageModel
        {
            Name = programmingLanguage.Name,
            Version = programmingLanguage.Version
        };
    }

    public static List<ProgrammingLanguageModel> ToModels(this IList<ProgrammingLanguage> programmingLanguages)
    {
        return ProgrammingLanguagesToModels(programmingLanguages);
    }

    public static List<ProgrammingLanguageModel> ToModels(this IEnumerable<ProgrammingLanguage> programmingLanguages)
    {
        return ProgrammingLanguagesToModels(programmingLanguages);
    }
}
