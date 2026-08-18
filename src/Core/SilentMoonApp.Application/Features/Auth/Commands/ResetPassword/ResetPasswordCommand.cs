using SilentMoonApp.Application.Abstractions.Messaging;

namespace SilentMoonApp.Application.Features.Auth.Commands.ResetPassword;

public sealed record ResetPasswordCommand(string Email,
										  string OtpCode,
										  string NewPassword,
										  string ConfirmPassword) : ICommand<ResetPasswordResult>,
																    INonTransactionalCommand,
																    INonLoggableCommand;
