namespace Tsa.Submissions.Coding.WebApi.Configuration;

public class EventSettings
{
    public const string SectionName = "Event";

    public int DurationInMinutes { get; set; }

    public EventSettingsConfigError GetError()
    {
        if (DurationInMinutes <= 0)
        {
            return EventSettingsConfigError.DurationInMinutes;
        }

        return EventSettingsConfigError.None;
    }

    public bool IsValid()
    {
        return DurationInMinutes > 0;
    }
}

public enum EventSettingsConfigError
{
    None = 0,
    DurationInMinutes = 1
}
