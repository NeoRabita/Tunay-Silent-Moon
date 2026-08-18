using SilentMoonApp.Application.Abstractions.Messaging;


namespace SilentMoonApp.Application.Features.Auth.Commands.FacebookAuth;

public sealed record FacebookAuthCommand(string IdToken) : ICommand<FacebookAuthResult>,
														   INonTransactionalCommand;