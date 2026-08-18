namespace SilentMoonApp.Application.Abstractions.Communication.Email;

public sealed class EmailMessage
{
	public EmailMessage(string to, string subject,
						string htmlBody, string? textBody = null)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(to);
		ArgumentException.ThrowIfNullOrWhiteSpace(subject);
		ArgumentException.ThrowIfNullOrWhiteSpace(htmlBody);


		To = to.Trim();

		Subject = subject.Trim();

		HtmlBody = htmlBody;

		TextBody = string.IsNullOrWhiteSpace(textBody)
				 ? null
				 : textBody.Trim();
	}


	public string To { get; }
	public string Subject { get; }
	public string HtmlBody { get; }
	public string? TextBody { get; }
}
