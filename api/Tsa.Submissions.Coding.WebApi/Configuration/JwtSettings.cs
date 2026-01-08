namespace Tsa.Submissions.Coding.WebApi.Configuration;

public class JwtSettings
{
    public string? Audience { get; set; }

    public int ExpirationInHours { get; set; }

    public string? Issuer { get; set; }

    public string? Key { get; set; }

    public bool RequireHttpsMetadata { get; set; } = true;

    public JwtSettingsConfigError GetError()
    {
        if (string.IsNullOrWhiteSpace(Audience))
        {
            return JwtSettingsConfigError.Audience;
        }

        if (ExpirationInHours <= 0)
        {
            return JwtSettingsConfigError.ExpirationInHours;
        }

        if (string.IsNullOrWhiteSpace(Issuer))
        {
            return JwtSettingsConfigError.Issuer;
        }

        if (string.IsNullOrWhiteSpace(Key))
        {
            return JwtSettingsConfigError.Key;
        }

        return JwtSettingsConfigError.None;
    }

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Audience) &&
               !string.IsNullOrWhiteSpace(Issuer) &&
               !string.IsNullOrWhiteSpace(Key) &&
               ExpirationInHours > 0;
    }
}

public enum JwtSettingsConfigError
{
    None = 0,
    Audience = 1,
    ExpirationInHours = 2,
    Issuer = 3,
    Key = 4
}
