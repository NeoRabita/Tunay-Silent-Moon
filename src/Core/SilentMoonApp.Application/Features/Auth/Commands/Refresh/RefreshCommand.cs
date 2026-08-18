using SilentMoonApp.Application.Abstractions.Messaging;

namespace SilentMoonApp.Application.Features.Auth.Commands.Refresh;

public sealed record RefreshCommand(string RefreshToken) : ICommand<RefreshResult>,
														   INonLoggableCommand;
