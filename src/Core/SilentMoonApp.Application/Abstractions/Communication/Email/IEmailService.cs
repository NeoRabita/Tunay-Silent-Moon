namespace SilentMoonApp.Application.Abstractions.Communication.Email;

public interface IEmailService
{
	Task SendAsync(EmailMessage emailMessage,
				   CancellationToken cancellationToken = default);
}
