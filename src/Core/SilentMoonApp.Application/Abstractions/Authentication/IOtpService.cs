using SilentMoonApp.Application.DTOs.Auth;

namespace SilentMoonApp.Application.Abstractions.Authentication;

public interface IOtpService
{
	GeneratedOtpResult GenerateVerificationCode();
}
