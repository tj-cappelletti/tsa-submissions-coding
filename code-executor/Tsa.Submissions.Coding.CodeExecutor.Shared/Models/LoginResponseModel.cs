using System.Text.Json.Serialization;

namespace Tsa.Submissions.Coding.CodeExecutor.Shared.Models;

public class LoginResponseModel
{
    [JsonPropertyName("expiration")]
    public DateTimeOffset Expiration { get; set; }

    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;
}
