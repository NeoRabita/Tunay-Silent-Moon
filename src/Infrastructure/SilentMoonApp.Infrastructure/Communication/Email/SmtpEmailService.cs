using MimeKit;
using MimeKit.Utils;
using MailKit.Net.Smtp;
using MailKit.Security;
using SilentMoonApp.Application.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SilentMoonApp.Application.Extensions;
using SilentMoonApp.Infrastructure.Settings;
using SilentMoonApp.Application.Abstractions.Logging;
using SilentMoonApp.Application.Abstractions.Communication.Email;


namespace SilentMoonApp.Infrastructure.Communication.Email;

public class SmtpEmailService : IEmailService, IAsyncDisposable
{
	private readonly MailSettings _mailSettings;
	private readonly ILogger<SmtpEmailService> _logger;
	private readonly ILogMasker _logMasker;

	private readonly SemaphoreSlim _lock = new(1, 1);
	private SmtpClient _smtpClient = new();


	public SmtpEmailService(IOptions<MailSettings> options,
							ILogger<SmtpEmailService> logger,
							ILogMasker logMasker)
	{
		ArgumentNullException.ThrowIfNull(options);
		ArgumentNullException.ThrowIfNull(logMasker);
		ArgumentNullException.ThrowIfNull(logger);


		_mailSettings = options.Value;
		_logger = logger;
		_logMasker = logMasker;
	}




	public async Task SendAsync(EmailMessage emailMessage,
									 CancellationToken ct = default)
	{
		using TimedOperation operation = _logger.BeginTimedOperation(operation: "Send Email",
																	 logLevel: LogLevel.Information);

		ArgumentNullException.ThrowIfNull(emailMessage);

		MimeMessage mimeMessage = GenerateMimeMessage(emailMessage);

		await _lock.WaitAsync(ct);

		//using var smtpClient = new SmtpClient();


		try
		{
			if (_smtpClient.IsConnected && _smtpClient.IsAuthenticated)
			{
				try
				{
					await _smtpClient.NoOpAsync();
				}

				catch
				{
					await _smtpClient.DisconnectAsync(quit: false,
													  cancellationToken: ct);
				}
			}


			if (!_smtpClient.IsConnected)
			{

				SecureSocketOptions socketOptions = GetSecureSocketOptions(_mailSettings.Port);

				await _smtpClient.ConnectAsync(host: _mailSettings.Host,
										  port: _mailSettings.Port,
										  options: socketOptions,
										  cancellationToken: ct);
			}


			if (!_smtpClient.IsAuthenticated &&
				!string.IsNullOrWhiteSpace(_mailSettings.UserName) &&
				!string.IsNullOrWhiteSpace(_mailSettings.Password))

				await _smtpClient.AuthenticateAsync(userName: _mailSettings.UserName,
													password: _mailSettings.Password,
													cancellationToken: ct);


			await _smtpClient.SendAsync(message: mimeMessage,
									   cancellationToken: ct);


			_logger.LogInformation(message: "Email SMTP server t?r?find?n q?bul edildi. " +
											"MessageId: {MessageId}," +
											"Recipient: {Recipient}",
								   mimeMessage.MessageId,
								   _logMasker.Mask(emailMessage.To));
		}

		catch (OperationCanceledException) when (ct.IsCancellationRequested)
		{
			_logger.LogWarning(message: "Email gönd?rilm?si l?gv edildi. " +
										"MessageId: {MessageId}," +
										"Recipient: {Recipient}",
							   mimeMessage.MessageId,
							   _logMasker.Mask(emailMessage.To));

			throw;
		}

		catch (SmtpCommandException exception)
		{
			_logger.LogError(
				exception,
				"""
				SMTP command rejected.
				ErrorCode: {ErrorCode}
				StatusCode: {StatusCode}
				Mailbox: {Mailbox}
				Recipient: [{Recipient}]
				""",
				exception.ErrorCode,
				exception.StatusCode,
				exception.Mailbox,
				_logMasker.Mask(emailMessage.To));

			throw;
		}

		finally
		{
			_lock.Release();
		}

	}


	public async ValueTask DisposeAsync()
	{
		try
		{
			if (_smtpClient.IsConnected)
				await _smtpClient.DisconnectAsync(quit: true,
												  cancellationToken: CancellationToken.None);
		}

		finally
		{
			_smtpClient.Dispose();
			_lock.Dispose();
		}
	}



	// Helpers 

	private MimeMessage GenerateMimeMessage(EmailMessage emailMessage)
	{
		MimeMessage mimeMessage = new()
		{
			MessageId = MimeUtils.GenerateMessageId(),
			Subject = emailMessage.Subject,
		};


		mimeMessage.From.Add(new MailboxAddress(_mailSettings.FromName,
												_mailSettings.FromAddress));

		string recipientEmail = emailMessage.To.Trim();

		if (!MailboxAddress.TryParse(recipientEmail, out MailboxAddress? recipient))
			throw new ArgumentException(message: "Recipient email address is not valid.",
										paramName: nameof(emailMessage.To));


		mimeMessage.To.Add(recipient);


		var bodyBuilder = new BodyBuilder
		{
			HtmlBody = emailMessage.HtmlBody,
			TextBody = emailMessage.TextBody
		};

		mimeMessage.Body = bodyBuilder.ToMessageBody();


		return mimeMessage;
	}


	private static SecureSocketOptions GetSecureSocketOptions(int port)
		=> port switch
		{
			1025 => SecureSocketOptions.None,
			587 => SecureSocketOptions.StartTls,
			465 => SecureSocketOptions.SslOnConnect,
			25 => SecureSocketOptions.StartTlsWhenAvailable,
			_ => SecureSocketOptions.Auto
		};
}
