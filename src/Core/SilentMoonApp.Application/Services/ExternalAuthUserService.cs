using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Abstractions.Repositories.Write;
using SilentMoonApp.Application.DTOs.Auth;
using SilentMoonApp.Application.Errors;


namespace SilentMoonApp.Application.Services;

public sealed class ExternalAuthUserService : IExternalAuthUserService
{
	private readonly TimeProvider _timeProvider;
	private readonly IUnitOfWork _unitOfWork;

	public ExternalAuthUserService(TimeProvider timeProvider,
								   IUnitOfWork unitOfWork)
	{
		_timeProvider = timeProvider;
		_unitOfWork = unitOfWork;
	}



	public async Task<Result<User>> GetOrGenerateActiveUserAsync(ExternalAuthProviderResult providerResult,
															   CancellationToken cancellationToken = default)
	{
		UserExternalProvider? userExternalProvider = await _unitOfWork.Repository<IUserExternalProviderReadRepository>().GetProviderWithUserAsync(
			provider: providerResult.Provider,
			providerUserId: providerResult.ProviderUserId,
			cancellationToken: cancellationToken);

		User user;


		if (userExternalProvider is not null)
		{
			user = userExternalProvider.User
				?? throw new InvalidOperationException("External provider user is null.");
		}

		else
		{
			Result<User> createUserResult = await GenerateExternalUserAsync(providerResult, cancellationToken);

			if (createUserResult.IsFailure)
				return Result<User>.Failure(createUserResult.Error);

			user = createUserResult.Value;
		}


		if (user.IsDeleted || user.UserStatus is not EUserStatus.Active)
			
			return Result<User>.Failure(
				ExternalAuthErrors.AccountUnavailable());


		return Result<User>.Success(user);
	}


	private async Task<Result<User>> GenerateExternalUserAsync(ExternalAuthProviderResult providerResult,
															 CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(providerResult.Email))
			return Result<User>.Failure(ExternalAuthErrors.EmailRequired());


		string normalizedEmail = providerResult.Email.Trim().ToLowerInvariant();


		bool isEmailAlreadyExists = await _unitOfWork.ReadRepository<User>().AnyAsync(filter: user => user.Email == normalizedEmail,
																					  cancellationToken: cancellationToken);

		if (isEmailAlreadyExists)
			return Result<User>.Failure(ExternalAuthErrors.AccountLinkRequired());


		DateTimeOffset nowUtc = _timeProvider.GetUtcNow();


		User user = new()
		{
			FirstName = string.IsNullOrWhiteSpace(providerResult.FirstName)
					  ? "User"
					  : providerResult.FirstName.Trim(),

			LastName = string.IsNullOrWhiteSpace(providerResult.LastName)
					 ? string.Empty
					 : providerResult.LastName.Trim(),

			UserName = null,
			PasswordHash = string.Empty,

			Email = normalizedEmail,
			IsEmailConfirmed = true,

			AccessFailedCount = 0,
			LockoutEndAt = null,

			UserStatus = EUserStatus.Active,

			CreatedAt = nowUtc,
			UpdatedAt = null,
			ConfirmedAt = nowUtc,

			IsDeleted = false,
		};


		await _unitOfWork.WriteRepository<User>().AddAsync(
			entity: user,
			cancellationToken: cancellationToken);


		UserExternalProvider newUserExternalProvider = new()
		{
			UserId = user.Id,
			User = user,
			Provider = providerResult.Provider,
			ProviderUserId = providerResult.ProviderUserId,
			CreatedAt = nowUtc,
		};

		await _unitOfWork.WriteRepository<UserExternalProvider>().AddAsync(entity: newUserExternalProvider,
																		   cancellationToken: cancellationToken);

		return Result<User>.Success(user);
	}

}
