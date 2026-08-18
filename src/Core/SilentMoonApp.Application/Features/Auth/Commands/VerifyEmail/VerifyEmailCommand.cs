using SilentMoonApp.Application.Abstractions.Messaging;

namespace SilentMoonApp.Application.Features.Auth.Commands.VerifyEmail;

public sealed record VerifyEmailCommand(string Email,
										string OtpCode) : ICommand<VerifyEmailResult>;
