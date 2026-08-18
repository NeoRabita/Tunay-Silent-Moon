using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Hashing;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Abstractions.Repositories.Write;
using SilentMoonApp.Application.DTOs.Auth;
using SilentMoonApp.Application.Errors;


namespace SilentMoonApp.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : ICommandHandler<RegisterCommand, RegisterResult>
{
	private readonly TimeProvider _timeProvider;
	private readonly IAuthOtpService _authOtpService;
	private readonly IPasswordHasher _passwordHasher;
	private readonly IUnitOfWork _unitOfWork;

	public RegisterCommandHandler(TimeProvider timeProvider,
								  IAuthOtpService authOtpService,
								  IPasswordHasher passwordHasher,
								  IUnitOfWork unitOfWork)
	{
		_timeProvider = timeProvider;
		_authOtpService = authOtpService;
		_passwordHasher = passwordHasher;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<RegisterResult>> Handle(RegisterCommand command,
													 CancellationToken ct = default)
	{
		string normalizerEmail = command.Email.Trim().ToLowerInvariant();

		string? userName = string.IsNullOrWhiteSpace(command.UserName)
						 ? null
						 : command.UserName.Trim();


		bool isEmailAlreadyExists = await _unitOfWork.ReadRepository<User>().AnyAsync(filter: user => user.Email == normalizerEmail,
																								 cancellationToken: ct);

		if (isEmailAlreadyExists)
			return Result<RegisterResult>.Failure(
				UserErrors.EmailAlreadyExsists());


		if (userName is not null)
		{
			bool isUserNameAlreadyExists = await _unitOfWork.ReadRepository<User>().AnyAsync(filter: user => user.UserName == userName,
																							 cancellationToken: ct);

			if (isUserNameAlreadyExists)
				return Result<RegisterResult>.Failure(
					UserErrors.UserNameAlreadyExists());
		}


		DateTimeOffset nowUtc = _timeProvider.GetUtcNow();


		User user = new()
		{
			FirstName = command.FirstName.Trim(),
			LastName = command.LastName?.Trim() ?? string.Empty,
			UserName = userName,

			Email = normalizerEmail,
			PasswordHash = _passwordHasher.Hash(command.Password),
			IsEmailConfirmed = false,

			AccessFailedCount = 0,
			LockoutEndAt = null,
			ConfirmedAt = null,

			UserStatus = EUserStatus.PendingVerification,

			CreatedAt = nowUtc,
			UpdatedAt = null,
		};

		await _unitOfWork.WriteRepository<User>().AddAsync(user, ct);

		await _unitOfWork.SaveChangesAsync(ct);


		GeneratedOtpResult generatedOtp = await _authOtpService.SendEmailConfirmationOtpAsync(user: user,
																						      requestEmail: normalizerEmail,
																						      cancellationToken: ct);

		return Result<RegisterResult>.Success(
			new RegisterResult
			(
				Message: $"Qeydiyyat uğurlu oldu. {normalizerEmail} E-poçtunuza göndərilən kodu daxil edin.",
				Email: normalizerEmail,
				OtpExpiresAt: generatedOtp.ExpiresAt
			)
		);
	}
}
