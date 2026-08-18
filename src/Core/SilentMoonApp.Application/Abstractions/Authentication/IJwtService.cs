using SilentMoonApp.Application.DTOs.Auth;
using SilentMoonApp.Domain.Entities.Identity;

namespace SilentMoonApp.Application.Abstractions.Authentication;

public interface IJwtService
{
	GeneratedAccessTokenResult GenerateAccessToken(User user);
	GeneratedRefreshTokenResult GenerateRefreshToken();
	string HashRefreshToken(string rawRefreshToken);
}
