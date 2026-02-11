namespace Tsa.Submissions.Coding.Contracts.Languages;

public record ProgrammingLanguageResponse
{
    public string FileExtension { get; set; }

    public string Id { get; set; }

    public string Identifier { get; set; }

    public bool IsEnabled { get; set; } = true;

    public string Name { get; set; } = string.Empty;

    public List<ProgrammingLanguageVersionResponse> Versions { get; set; } = [];

    public ProgrammingLanguageResponse(
        string id,
        string identifier,
        string name,
        string fileExtension,
        bool isEnabled,
        IEnumerable<ProgrammingLanguageVersionResponse> versions)
    {
        FileExtension = fileExtension;
        Id = id;
        Identifier = identifier;
        IsEnabled = isEnabled;
        Name = name;
        Versions = versions.ToList();
    }
}
