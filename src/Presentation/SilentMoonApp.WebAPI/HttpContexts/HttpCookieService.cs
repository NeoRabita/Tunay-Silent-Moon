using Microsoft.Extensions.Options;

namespace SilentMoonApp.WebAPI.HttpContexts;

public class HttpCookieService
{
	private readonly IOptions<HttpCookieSettings> _options;
	private readonly TimeProvider _timeProvider;

	public HttpCookieService(IOptions<HttpCookieSettings> options,
							 TimeProvider timeProvider)
	{
		_options = options;
		_timeProvider = timeProvider;
	}



	public void Set(HttpResponse response, string value, DateTimeOffset expiresAt)
	{
		ArgumentNullException.ThrowIfNull(response);
		ArgumentNullException.ThrowIfNullOrWhiteSpace(value);

		DateTimeOffset nowUtc = _timeProvider.GetUtcNow();
		DateTimeOffset normalizedExpiration = expiresAt.ToUniversalTime();

		if (normalizedExpiration <= nowUtc)
			throw new ArgumentOutOfRangeException(nameof(expiresAt), expiresAt, "The expiration date must be in the future.");

		CookieOptions cookieOptions = CreateBaseOptions();

		cookieOptions.Expires = normalizedExpiration;
		cookieOptions.MaxAge = normalizedExpiration - nowUtc;

		response.Cookies.Append(_options.Value.Name, value, cookieOptions);
	}


	public bool TryGet(HttpRequest request, out string? value)
	{
		ArgumentNullException.ThrowIfNull(request);

		if (!request.Cookies.TryGetValue(_options.Value.Name, out string? cookieValue) ||
			 string.IsNullOrWhiteSpace(cookieValue))
		{
			value = null;
			return false;
		}

		value = cookieValue;
		return true;
	}


	public void Delete(HttpResponse response)
	{
		ArgumentNullException.ThrowIfNull(response);

		CookieOptions cookieOptions = CreateBaseOptions();

		response.Cookies.Delete(_options.Value.Name, cookieOptions);
	}



	// Helpers

	private CookieOptions CreateBaseOptions()
		=> new CookieOptions
		{
			HttpOnly = true,
			Secure = true,
			IsEssential = true,

			Path = _options.Value.Path,
			SameSite = _options.Value.SameSite,
			//Domain = _options.Value.Domain
		};

}
