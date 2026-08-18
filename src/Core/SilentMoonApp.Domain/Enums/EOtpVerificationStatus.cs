namespace SilentMoonApp.Domain.Enums;

public enum EOtpVerificationStatus
{
	Succeeded = 1,
	InvalidCode = 2,
	Unavailable = 3,
	AttemptsExceeded = 4
}
