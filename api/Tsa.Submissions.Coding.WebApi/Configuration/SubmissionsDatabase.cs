namespace Tsa.Submissions.Coding.WebApi.Configuration;

public class SubmissionsDatabase
{
    public string? Host { get; set; }

    public string? LoginDatabase { get; set; }

    public string? Name { get; set; }

    public string? Password { get; set; }

    public int Port { get; set; }

    public string? Username { get; set; }

    public SubmissionsDatabaseConfigError GetError()
    {
        if (string.IsNullOrWhiteSpace(Host))
        {
            return SubmissionsDatabaseConfigError.Host;
        }

        if (string.IsNullOrWhiteSpace(LoginDatabase))
        {
            return SubmissionsDatabaseConfigError.LoginDatabase;
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            return SubmissionsDatabaseConfigError.Name;
        }

        if (string.IsNullOrWhiteSpace(Username))
        {
            return SubmissionsDatabaseConfigError.Username;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            return SubmissionsDatabaseConfigError.Password;
        }

        if (Port <= 0)
        {
            return SubmissionsDatabaseConfigError.Port;
        }

        return SubmissionsDatabaseConfigError.None;
    }

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Host) &&
               !string.IsNullOrWhiteSpace(LoginDatabase) &&
               !string.IsNullOrWhiteSpace(Name) &&
               !string.IsNullOrWhiteSpace(Username) &&
               !string.IsNullOrWhiteSpace(Password) &&
               Port > 0;
    }
}

public enum SubmissionsDatabaseConfigError
{
    None = 0,
    Host = 1,
    LoginDatabase = 2,
    Name = 3,
    Password = 4,
    Port = 5,
    Username = 6
}
