namespace SilentMoonApp.Domain.Enums;

public enum ERevocationReason
{
	UserLogout = 1,
	PasswordChanged = 2,
	AdminRevoked = 3,
	ReuseDetected = 4,
	SuspiciousActivity = 5
}
