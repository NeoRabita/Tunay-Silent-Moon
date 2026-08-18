using SilentMoonApp.Application.Abstractions.Messaging;

namespace SilentMoonApp.Application.Features.Auth.Commands.ResendEmailOtp;

public sealed record ResendEmailOtpCommand(string Email) : ICommand<ResendEmailOtpResult>,
														   INonTransactionalCommand;
