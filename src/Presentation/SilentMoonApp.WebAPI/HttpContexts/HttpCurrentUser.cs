using SilentMoonApp.Application.Abstractions.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SilentMoonApp.WebAPI.HttpContexts;

public class HttpCurrentUser : ICurrentUser
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public HttpCurrentUser(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}



	private ClaimsPrincipal? User
		  => _httpContextAccessor.HttpContext?.User;


	public bool IsAuthenticated
		  => User?.Identity?.IsAuthenticated == true;


	public string? Email
		  => User?.FindFirstValue(JwtRegisteredClaimNames.Email)
		  ?? User?.FindFirstValue(ClaimTypes.Email);


	public string? UserName
		  => User?.FindFirstValue(JwtRegisteredClaimNames.UniqueName)
		  ?? User?.FindFirstValue(ClaimTypes.Name);


	public Guid? UserId
	{
		get
		{
			string? userIdClaim = User?.FindFirstValue(JwtRegisteredClaimNames.Sub)
							   ?? User?.FindFirstValue(ClaimTypes.NameIdentifier);

			return Guid.TryParse(userIdClaim, out var userId)
				? userId
				: null;
		}
	}


	public IReadOnlyCollection<string> Roles
		=> User?.FindAll(ClaimTypes.Role)
				.Select(role => role.Value)
				.Where(role => !string.IsNullOrWhiteSpace(role))
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.ToArray()
		?? Array.Empty<string>();

}
