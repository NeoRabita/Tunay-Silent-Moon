using StackExchange.Redis;
using SilentMoonApp.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SilentMoonApp.Application.Helpers;
using SilentMoonApp.Application.Settings;
using SilentMoonApp.Application.DTOs.Auth;
using SilentMoonApp.Application.Extensions;
using SilentMoonApp.Application.Abstractions.Caching;
using SilentMoonApp.Application.Abstractions.Hashing;


namespace SilentMoonApp.Infrastructure.Caching;

public class RedisOtpCacheService : IOtpCacheService
{
	private const string VerifyScript =
			"""
		-- OTP Redis-də yoxdursa istifadə edilə bilməz.
		if redis.call('EXISTS', KEYS[1]) == 0 then
			return { -1, 0 }
		end


		local storedHash =
			redis.call('HGET', KEYS[1], 'codeHash')


		local failedAttempts =
			tonumber(
				redis.call(
					'HGET',
					KEYS[1],
					'failedAttempts'
				) or '0'
			)


		local maxAttempts =
			tonumber(ARGV[2])


		-- Limit artiq tamamlanibsa OTP silinir.
		if failedAttempts >= maxAttempts then
			redis.call('DEL', KEYS[1])

			return { -2, 0 }
		end


		-- OTP dogrudursa həmin anda silinir.
		if storedHash == ARGV[1] then
			redis.call('DEL', KEYS[1])

			return { 1, 0 }
		end


		-- Yanlis cəhd atomik şəkildə artırılır.
		failedAttempts =
			redis.call(
				'HINCRBY',
				KEYS[1],
				'failedAttempts',
				1
			)


		-- 5-ci yanlis cəhddə OTP silinir.
		if failedAttempts >= maxAttempts then
			redis.call('DEL', KEYS[1])

			return { -2, 0 }
		end


		return {
			0,
			maxAttempts - failedAttempts
		}
		""";


	private readonly IDatabase _database;
	private readonly IOtpHasher _otpHasher;
	private readonly TimeProvider _timeProvider;
	private readonly OtpSettings _settings;
	private readonly ILogger<RedisOtpCacheService> _logger;



	public RedisOtpCacheService(IConnectionMultiplexer connectionMultiplexer,
								IOtpHasher otpHasher,
								TimeProvider timeProvider,
								IOptions<OtpSettings> options,
								ILogger<RedisOtpCacheService> logger)
	{
		_database = connectionMultiplexer.GetDatabase();
		_otpHasher = otpHasher;
		_timeProvider = timeProvider;
		_settings = options.Value;
		_logger = logger;
	}



	public async Task StoreOtpAsync(Guid userId,
							  EOtpPurpose otpPurpose,
							  string rawCode,
							  DateTimeOffset expiresAt,
							  CancellationToken ct = default)
	{
		using TimedOperation operation = _logger.BeginTimedOperation(operation: $"Store OTP",
																	 logLevel: LogLevel.Debug);

		ArgumentException.ThrowIfNullOrWhiteSpace(argument: rawCode,
												  paramName: nameof(rawCode));


		string key = OtpCacheKeys.GenerateKey(userId: userId,
											  otpPurpose: otpPurpose);

		string codeHash = _otpHasher.Hash(rawCode);

		TimeSpan remainingTime = expiresAt - _timeProvider.GetUtcNow();


		if (remainingTime <= TimeSpan.Zero)
			throw new ArgumentException(message: "The expiration time must be in the future.",
										paramName: nameof(expiresAt));


		await _database.HashSetAsync(key: key,
									 hashFields: [
										 new HashEntry(name: "codeHash", value: codeHash),
										 new HashEntry(name: "failedAttempts", value: 0)
												])
					   .WaitAsync(ct);

		await _database.KeyExpireAsync(key: key,
									   expiry: remainingTime)
					   .WaitAsync(ct);
	}


	public async Task<OtpCacheVerificationResult> VerifyOtpAsync(Guid userId, EOtpPurpose otpPurpose, string rawCode, CancellationToken ct = default)
	{
		using TimedOperation operation = _logger.BeginTimedOperation(operation: $"Verify OTP",
																	 logLevel: LogLevel.Debug);

		ArgumentException.ThrowIfNullOrWhiteSpace(argument: rawCode,
												  paramName: nameof(rawCode));


		string key = OtpCacheKeys.GenerateKey(userId: userId,
											  otpPurpose: otpPurpose);

		string submittedHash = _otpHasher.Hash(rawCode);


		RedisResult scriptResult = await _database.ScriptEvaluateAsync(script: VerifyScript,
																	   keys: [
																	  			(RedisKey)key
																			   ],
																	   values: [
																					(RedisValue)submittedHash,
																					(RedisValue)_settings.MaxFailedAttempts,
																				 ])
											.WaitAsync(ct);

		RedisResult[] values = (RedisResult[])scriptResult!;

		long statusCode = (long)values[0];

		int remainingAttempts = (int)(long)values[1];


		return statusCode switch
		{
			1 => new OtpCacheVerificationResult
			(
				OtpVerificationStatus: EOtpVerificationStatus.Succeeded,
				RemainingAttempts: remainingAttempts
			),

			0 => new OtpCacheVerificationResult
			(
				OtpVerificationStatus: EOtpVerificationStatus.InvalidCode,
				RemainingAttempts: remainingAttempts
			),

			-1 => new OtpCacheVerificationResult
			(
				OtpVerificationStatus: EOtpVerificationStatus.Unavailable,
				RemainingAttempts: 0
			),

			-2 => new OtpCacheVerificationResult
			(
				OtpVerificationStatus: EOtpVerificationStatus.AttemptsExceeded,
				RemainingAttempts: 0
			),

			_ => throw new InvalidOperationException(message: $"Unexpected status code returned from Redis script: {statusCode}")
		};

	}


	public Task<bool> RemoveOtpAsync(Guid userId, EOtpPurpose otpPurpose, CancellationToken ct = default)
	{
		using TimedOperation operation = _logger.BeginTimedOperation(operation: $"Remove OTP",
																	 logLevel: LogLevel.Debug);

		string key = OtpCacheKeys.GenerateKey(userId: userId,
											  otpPurpose: otpPurpose);


		return _database.KeyDeleteAsync(key: key)
						.WaitAsync(ct);
	}

}
