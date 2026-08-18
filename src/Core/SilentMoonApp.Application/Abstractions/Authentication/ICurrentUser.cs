namespace SilentMoonApp.Application.Abstractions.Authentication;

public interface ICurrentUser
{
	bool IsAuthenticated { get; }
	Guid? UserId { get; }
	string? Email { get; }
	string? UserName { get; }
	IReadOnlyCollection<string> Roles { get; }
}
