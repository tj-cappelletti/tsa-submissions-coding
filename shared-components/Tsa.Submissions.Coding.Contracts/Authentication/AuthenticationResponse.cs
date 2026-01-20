namespace Tsa.Submissions.Coding.Contracts.Authentication;

public record AuthenticationResponse(DateTimeOffset Expiration, string Token);
