using System.Text;
using RabbitMQ.Client;
using System.Text.Json;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SilentMoonApp.Infrastructure.Settings;
using SilentMoonApp.Application.Exceptions.Email;
using SilentMoonApp.Application.Abstractions.Communication.Email;


namespace SilentMoonApp.Infrastructure.Communication.Email;

public class RabbitMqEmailConsumerWorker : BackgroundService
{
	private readonly RabbitMqSettings _settings;
	private readonly IEmailService _emailService;
	private readonly ILogger<RabbitMqEmailConsumerWorker> _logger;

	private IConnection? _connection;
	private IChannel? _channel;


	public RabbitMqEmailConsumerWorker(IOptions<RabbitMqSettings> options,
									   [FromKeyedServices("smtp")] IEmailService emailService,
									   ILogger<RabbitMqEmailConsumerWorker> logger)
	{
		_settings = options.Value;
		_emailService = emailService;
		_logger = logger;
	}


	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		ConnectionFactory factory = new()
		{
			HostName = _settings.Host,
			Port = _settings.Port,
			UserName = _settings.UserName,
			Password = _settings.Password,
			VirtualHost = _settings.VirtualHost
		};


		_connection = await factory.CreateConnectionAsync(cancellationToken: stoppingToken);
		_channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);


		await _channel.QueueDeclareAsync(queue: _settings.QueueName,
										 durable: true,
										 exclusive: false,
										 autoDelete: false,
										 arguments: null,
										 cancellationToken: stoppingToken);

		await _channel.BasicQosAsync(prefetchSize: 0,
									 prefetchCount: 1,
									 global: false,
									 cancellationToken: stoppingToken);


		AsyncEventingBasicConsumer consumer = new(_channel);


		consumer.ReceivedAsync += async (_, eventArgs) =>
		{
			try
			{
				string json = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

				EmailMessage? emailMessage = JsonSerializer.Deserialize<EmailMessage>(json);


				if (emailMessage is null)
				{
					await _channel.BasicRejectAsync(deliveryTag: eventArgs.DeliveryTag,
													requeue: false,
													cancellationToken: stoppingToken);

					return;
				}


				EmailMessage emailMessage2 = new
				(
					to: emailMessage.To,
					subject: emailMessage.Subject,
					htmlBody: emailMessage.HtmlBody,
					textBody: emailMessage.TextBody
				);


				await _emailService.SendAsync(emailMessage2, stoppingToken);

				await _channel.BasicAckAsync(deliveryTag: eventArgs.DeliveryTag,
											 multiple: false,
											 cancellationToken: stoppingToken);
			}

			catch (SmtpTransientException exception)
			{
				_logger.LogError(exception: exception,
								 message: "Email müv?qq?ti s?b?b? gör? gönd?ril? bilm?di. Yenid?n c?hd edil?c?k.");

				await _channel.BasicNackAsync(deliveryTag: eventArgs.DeliveryTag,
											  multiple: false,
											  requeue: true,
											  cancellationToken: stoppingToken);
			}

			catch (SmtpPermanentException exception)
			{
				_logger.LogError(exception: exception,
								 message: "Email permanent s?b?b? gör? gönd?ril? bilm?di.");

				await _channel.BasicRejectAsync(deliveryTag: eventArgs.DeliveryTag,
												requeue: false,
												cancellationToken: stoppingToken);
			}
		};


		await _channel.BasicConsumeAsync(queue: _settings.QueueName,
										 autoAck: false,
										 consumer: consumer,
										 cancellationToken: stoppingToken);

		await Task.Delay(delay: Timeout.InfiniteTimeSpan,
						 cancellationToken: stoppingToken);
	}


	public override async Task StopAsync(CancellationToken ct)
	{
		if (_channel is not null)
		{
			await _channel.CloseAsync(cancellationToken: ct);
			await _channel.DisposeAsync();
		}

		if (_connection is not null)
		{
			await _connection.CloseAsync(cancellationToken: ct);
			await _connection.DisposeAsync();
		}

		await base.StopAsync(cancellationToken: ct);
	}

}