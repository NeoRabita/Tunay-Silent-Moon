using Microsoft.Extensions.Options;
using SilentMoonApp.Application.Settings;
using System.Security.Cryptography;
using SilentMoonApp.Application.DTOs.Auth;
using SilentMoonApp.Application.Abstractions.Authentication;
using Microsoft.Extensions.Logging;
using SilentMoonApp.Application.Helpers;
using SilentMoonApp.Application.Extensions;


namespace SilentMoonApp.Infrastructure.Authentication;

public class OtpService : IOtpService
{
	private const int MinLength = 4;
	private const int MaxLength = 10;

	private readonly OtpSettings _settings;
	private readonly TimeProvider _timeProvider;
	private readonly ILogger<OtpService> _logger;


	public OtpService(TimeProvider timeProvider,
					  IOptions<OtpSettings> options,
					  ILogger<OtpService> logger)
	{
		_logger = logger;
		_settings = options.Value;
		_timeProvider = timeProvider;

	}



	public GeneratedOtpResult GenerateVerificationCode()
	{
		using TimedOperation operation = _logger.BeginTimedOperation(operation:"GenerateVerificationCode",
																	 logLevel: LogLevel.Debug);

		int otpLength = _settings.Length;
		DateTimeOffset nowUtc = _timeProvider.GetUtcNow();

		if (otpLength is < MinLength or > MaxLength)
			throw new ArgumentOutOfRangeException($"OTP length must be between {MinLength} and {MaxLength}.");

		Span<char> digits = stackalloc char[otpLength];
		//char[] digits = new char[length];

		for (int i = 0; i < otpLength; i++)
		{
			int randomDiigit = RandomNumberGenerator.GetInt32(0, 10);

			digits[i] = (char)('0' + randomDiigit);
		}

		string raw = new(digits);

		DateTimeOffset expiresAt = _timeProvider.GetUtcNow()
												.AddMinutes(_settings.ExpirationMinutes);

		return new GeneratedOtpResult(RawCode: raw,
									  ExpiresAt: expiresAt,
									  LifeTimeMinutes: _settings.ExpirationMinutes);
	}

}
