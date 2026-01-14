namespace Tsa.Submissions.Coding.WebApi.Configuration;

public class RabbitMQConfig
{
    public const string SectionName = "RabbitMQ";

    public string HostName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string Port { get; set; } = string.Empty;

    public string QueueName { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;
}
