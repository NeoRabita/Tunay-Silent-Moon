namespace SilentMoonApp.Application.Abstractions.Authentication;

public interface IRequestContext
{
	string? IpAddress { get; }
	string? UserAgent { get; }
	string? TraceId { get; }

	string? HttpMethod { get; }
	string? Path { get; }
}
