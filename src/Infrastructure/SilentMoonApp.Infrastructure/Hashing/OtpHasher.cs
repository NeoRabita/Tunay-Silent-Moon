using Microsoft.Extensions.Options;
using SilentMoonApp.Application.Abstractions.Hashing;
using SilentMoonApp.Application.Settings;
using System.Security.Cryptography;
using System.Text;

namespace SilentMoonApp.Infrastructure.Hashing;

public class OtpHasher : IOtpHasher
{
	private readonly OtpSettings _settings;
	private readonly byte[] _secretKey;

	public OtpHasher(IOptions<OtpSettings> options)
	{

		_settings = options.Value;
		_secretKey = Encoding.UTF8.GetBytes(_settings.OtpHmacKey);
	}



	public string Hash(string rawCode)
	{
		ArgumentException.ThrowIfNullOrEmpty(rawCode,
											 nameof(rawCode));

		byte[] otpBytes = Encoding.UTF8.GetBytes(rawCode);

		using var hmac = new HMACSHA256(_secretKey);

		byte[] hashBytes = hmac.ComputeHash(otpBytes);

		return Convert.ToBase64String(hashBytes);
	}


	public bool Verify(string rawOtpCode, string hashedOtpCode)
	{
		ArgumentException.ThrowIfNullOrEmpty(rawOtpCode,
											 nameof(rawOtpCode));

		ArgumentException.ThrowIfNullOrEmpty(hashedOtpCode,
											 nameof(hashedOtpCode));

		try
		{
			byte[] hashedOtpCodeBytes = Convert.FromBase64String(hashedOtpCode);


			string newHashCodeString = Hash(rawOtpCode);

			byte[] newHashCodeBytes = Convert.FromBase64String(newHashCodeString);


			return CryptographicOperations.FixedTimeEquals(hashedOtpCodeBytes,
														   newHashCodeBytes);
		}

		catch (FormatException)
		{
			return false;
		}

	}

}
