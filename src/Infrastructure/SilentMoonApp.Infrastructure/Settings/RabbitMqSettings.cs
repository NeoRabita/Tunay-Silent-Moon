namespace SilentMoonApp.Infrastructure.Settings;

public class RabbitMqSettings
{
	public const string SectionName = "RabbitMqSettings";

	public string Host { get; set; } = string.Empty;
	public int Port { get; set; } = 5672;
	public string UserName { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public string VirtualHost { get; set; } = "/";
	public string QueueName { get; set; } = "email-notifications";
}
