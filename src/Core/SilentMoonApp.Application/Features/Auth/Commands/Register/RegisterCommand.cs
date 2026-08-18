using SilentMoonApp.Application.Abstractions.Messaging;

namespace SilentMoonApp.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(string FirstName,
									 string? LastName,
									 string? UserName,
									 string Email,
									 string Password) : ICommand<RegisterResult>,
														INonTransactionalCommand;
