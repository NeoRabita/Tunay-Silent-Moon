namespace SilentMoonApp.WebAPI.Contracts.Auth.Register;

public sealed class RegisterRequest
{
	public required string FirstName { get; init; }
	public required string LastName { get; init; }
	public string? UserName { get; init; }
	public required string Email { get; init; }
	public required string Password { get; init; }
}
