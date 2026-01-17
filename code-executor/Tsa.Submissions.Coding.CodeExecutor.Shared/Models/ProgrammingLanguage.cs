using System.Diagnostics;

namespace Tsa.Submissions.Coding.CodeExecutor.Shared.Models;

[DebuggerDisplay("{Name} ({Version})")]
public class ProgrammingLanguage
{
    public string Name { get; set; } = string.Empty;

    public string Version { get; set; } = string.Empty;
}
