using SilentMoonApp.Application.Messaging;
using SilentMoonApp.Application.Abstractions.Messaging;

namespace SilentMoonApp.Application.Features.Auth.Commands.Logout;

public sealed record LogoutCommand(string RefreshToken) : ICommand<NoResult>, 
														  INonLoggableCommand;
