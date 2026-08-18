using SilentMoonApp.Application.Errors;
using SilentMoonApp.Application.Abstractions.Storage;
using SilentMoonApp.Application.Abstractions.Hashing;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.Abstractions.Repositories;



namespace SilentMoonApp.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResult>
{
	private const int MaxFailedAttempts = 5;
	private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

	private readonly TimeProvider _timeProvider;
	private readonly IAuthTokenService _authTokenService;
	private readonly IPasswordHasher _passwordHasher;
	private readonly IUserAvatarService _userAvatarService;
	private readonly IUnitOfWork _unitOfWork;

	public LoginCommandHandler(TimeProvider timeProvider,
							   IAuthTokenService authTokenService,
							   IPasswordHasher passwordHasher,
							   IUserAvatarService userAvatarService,
							   IUnitOfWork unitOfWork)
	{
		_timeProvider = timeProvider;
		_authTokenService = authTokenService;
		_passwordHasher = passwordHasher;
		_userAvatarService = userAvatarService;
		_unitOfWork = unitOfWork;
	}


	public async Task<Result<LoginResult>> Handle(LoginCommand command,
												  CancellationToken ct = default)
	{
		DateTimeOffset nowUtc = _timeProvider.GetUtcNow();
		
		string normalizedEmail = command.Email.Trim().ToLowerInvariant();


		User? user = await _unitOfWork.Repository<IUserReadRepository>().GetByEmailAsync(normalizedEmail,
																						 tracking: true,
																						 cancellationToken: ct);


		if (user is null)
			return Result<LoginResult>.Failure(AuthErrors.InvalidCredentials());

		if (user.LockoutEndAt.HasValue && user.LockoutEndAt.Value > nowUtc)
			return Result<LoginResult>.Failure(AuthErrors.AccountBlocked(user.LockoutEndAt.Value));


		bool isPasswordValid = _passwordHasher.Verify(command.Password, user.PasswordHash);


		if (!isPasswordValid)
		{
			user.AccessFailedCount++;

			if (user.AccessFailedCount >= MaxFailedAttempts)
			{
				user.LockoutEndAt = nowUtc.Add(LockoutDuration);

				return Result<LoginResult>.Failure(AuthErrors.AccountBlocked(user.LockoutEndAt.Value));
			}

			return Result<LoginResult>.Failure(
				AuthErrors.InvalidCredentials());
		}

		
		if (!user.IsEmailConfirmed || user.UserStatus is EUserStatus.PendingVerification)
			return Result<LoginResult>.Failure(
				AuthErrors.EmailNotVerified());

		if (user.UserStatus is not EUserStatus.Active)
			return Result<LoginResult>.Failure(
				AuthErrors.InvalidCredentials());


		user.AccessFailedCount = 0;
		user.LockoutEndAt = null;
		user.UpdatedAt = nowUtc;


		var session = await _authTokenService.GenerateSessionAsync(user, ct);

		string avatarUrl = await _userAvatarService.GetAvatarUrlAsync(user.AvatarImageFileId, ct);


		return Result<LoginResult>.Success(
			new LoginResult
			(
				AccessToken: session.AccessToken,
				RawRefreshToken: session.RefreshToken,
				TokenType: session.TokenType,
				AccessTokenExpiresIn: session.AccessTokenExpiresIn,
				RefreshTokenExpiresAt: session.RefreshTokenExpiresAt,

				User: new LoginUserResult(
					Id: user.Id,
					FirstName: user.FirstName,
					Email: user.Email,
					IsEmailVerified: user.IsEmailConfirmed,
					AvatarUrl: avatarUrl,
					CreatedAt: user.CreatedAt)
			)
		);
	}
}
