using SilentMoonApp.Application.Abstractions.Authentication;
using System.Diagnostics;

namespace SilentMoonApp.WebAPI.HttpContexts;

public class HttpRequestContext : IRequestContext
{
	public const int MaxUserAgentLength = 512;
	private readonly IHttpContextAccessor _httpContextAccessor;

	public HttpRequestContext(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}


	private HttpContext? Context => _httpContextAccessor.HttpContext;


	public string? IpAddress => Context?.Connection
											.RemoteIpAddress?
											.ToString();

	public string? UserAgent => Context?.Request
										.Headers
										.UserAgent
										.ToString();

	public string? TraceId => Activity.Current?.TraceId.ToString()
						   ?? Context?.TraceIdentifier;


	public string? HttpMethod => Context?.Request.Method;



	public string? Path => Context?.Request.Path.Value;

}
