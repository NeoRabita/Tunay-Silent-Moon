using SilentMoonApp.Application.Abstractions.Messaging;


namespace SilentMoonApp.Application.Features.Auth.Commands.GoogleAuth;

public sealed record GoogleAuthCommand(string IdToken) : ICommand<GoogleAuthResult>,
														 INonTransactionalCommand;



