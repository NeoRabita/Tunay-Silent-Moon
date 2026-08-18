using SilentMoonApp.Application.Abstractions.Messaging;

namespace SilentMoonApp.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(string Email,
								  string Password) : ICommand<LoginResult>,
													 INonTransactionalCommand;
