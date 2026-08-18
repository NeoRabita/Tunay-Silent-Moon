using SilentMoonApp.Application.Abstractions.Messaging;
using System.Windows.Input;

namespace SilentMoonApp.Application.Features.Auth.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : ICommand<ForgotPasswordResult>,
														   INonTransactionalCommand,
														   INonLoggableCommand;
