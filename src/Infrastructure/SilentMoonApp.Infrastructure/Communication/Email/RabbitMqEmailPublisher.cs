using System.Text;
using RabbitMQ.Client;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SilentMoonApp.Infrastructure.Settings;
using SilentMoonApp.Application.Abstractions.Logging;
using SilentMoonApp.Application.Abstractions.Communication.Email;


namespace SilentMoonApp.Infrastructure.Communication.Email;

public class RabbitMqEmailPublisher : IEmailService, IAsyncDisposable
{
	private readonly RabbitMqSettings _settings;
	private readonly ILogger<RabbitMqEmailPublisher> _logger;
	private readonly ILogMasker _logMasker;

	private readonly SemaphoreSlim _connectionLock = new(1, 1);
	private IConnection? _connection;


	public RabbitMqEmailPublisher(IOptions<RabbitMqSettings> options,
								  ILogger<RabbitMqEmailPublisher> logger,
								  ILogMasker logMasker)
	{
		_settings = options.Value;
		_logger = logger;
		_logMasker = logMasker;
	}

	public async Task SendAsync(EmailMessage emailMessage,
								CancellationToken ct = default)
	{
		ArgumentException.ThrowIfNullOrEmpty(emailMessage.To, nameof(emailMessage.To));


		if (_connection is null || !_connection.IsOpen)
		{
			await _connectionLock.WaitAsync(ct);

			try
			{
				if (_connection is null || !_connection.IsOpen)
				{
					_connection?.Dispose();

					ConnectionFactory factory = new()
					{
						HostName = _settings.Host,
						Port = _settings.Port,
						UserName = _settings.UserName,
						Password = _settings.Password,
						VirtualHost = _settings.VirtualHost
					};

					_connection = await factory.CreateConnectionAsync(cancellationToken: ct);
				}
			}

			finally
			{
				_connectionLock.Release();
			}
		}




		//await using IConnection connection = await factory.CreateConnectionAsync(cancellationToken: ct);

		await using IChannel channel = await _connection.CreateChannelAsync(cancellationToken: ct);

		await channel.QueueDeclareAsync(queue: _settings.QueueName,
										durable: true,
										exclusive: false,
										autoDelete: false,
										arguments: null,
										cancellationToken: ct);

		EmailMessage message = new
		(
			emailMessage.To,
			emailMessage.Subject,
			emailMessage.HtmlBody,
			emailMessage.TextBody
		);


		byte[] body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));


		BasicProperties properties = new()
		{
			Persistent = true,
			ContentType = "application/json"
		};


		await channel.BasicPublishAsync(exchange: string.Empty,
										routingKey: _settings.QueueName,
										mandatory: false,
										basicProperties: properties,
										body: body,
										cancellationToken: ct);


		_logger.LogInformation(message: "Email message sent to RabbitMQ queue '{QueueName}' for recipient '{Recipient}'",
							   args: [_settings.QueueName, _logMasker.Mask(emailMessage.To)]);
	}


	public async ValueTask DisposeAsync()
	{
		try
		{
			if (_connection is not null)
				await _connection!.DisposeAsync();

		}

		finally
		{
			_connectionLock.Dispose();
		}
	}

}
