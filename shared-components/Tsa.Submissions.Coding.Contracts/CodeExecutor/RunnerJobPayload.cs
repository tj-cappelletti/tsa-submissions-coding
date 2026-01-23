using Tsa.Submissions.Coding.Contracts.TestCases;

namespace Tsa.Submissions.Coding.Contracts.CodeExecutor;

public record RunnerJobPayload(
    string Language,
    string LanguageVersion,
    string ProblemId,
    string Solution,
    string SubmissionId,
    List<TestCase> TestCases);
