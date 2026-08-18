using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.DTOs.Auth;
using SilentMoonApp.Application.Errors;
using SilentMoonApp.Application.Exceptions.Auth.ExternalAuth;
using SilentMoonApp.Domain.Enums;
using SilentMoonApp.Infrastructure.Settings;
using SilentMoonApp.SharedKernel.Primitives;


namespace SilentMoonApp.Infrastructure.Authentication.Providers;

public class GoogleAuthProvider : IExternalAuthProvider
{
	private readonly GoogleAuthSettings _settings;

	public GoogleAuthProvider(IOptions<GoogleAuthSettings> options)
	{
		_settings = options.Value;
	}


	public EExternalAuthProvider Provider => EExternalAuthProvider.Google;


	public async Task<Result<ExternalAuthProviderResult>> VerifyAsync(string providerToken,
															  CancellationToken ct = default)
	{
		if(string.IsNullOrWhiteSpace(providerToken))
			return Result<ExternalAuthProviderResult>.Failure(
				ExternalAuthErrors.InvalidProviderToken(externalProvider: EExternalAuthProvider.Google));

		try
		{
			string clientId = _settings.ClientId;

			var validationSettings =
				new GoogleJsonWebSignature.ValidationSettings();

			validationSettings.Audience =
			[
				clientId
			];


			GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(jwt: providerToken,
																								validationSettings: validationSettings)
																				 .WaitAsync(ct);

			if (string.IsNullOrWhiteSpace(payload.Subject))
				return Result<ExternalAuthProviderResult>.Failure(
					ExternalAuthErrors.InvalidProviderToken(externalProvider: EExternalAuthProvider.Google));

			if (string.IsNullOrWhiteSpace(payload.Email) || !payload.EmailVerified)
				return Result<ExternalAuthProviderResult>.Failure(
					ExternalAuthErrors.EmailRequired());


			return Result<ExternalAuthProviderResult>.Success(
				new ExternalAuthProviderResult
				(
					Provider: EExternalAuthProvider.Google,
					ProviderUserId: payload.Subject,
					Email: payload.Email,
					FirstName: payload.GivenName,
					LastName: payload.FamilyName,
					AvatarUrl: payload.Picture
				));
		}

		catch (Exception exception) 
			when (exception is InvalidJwtException 
							or FormatException 
							or ArgumentException)              
		{
			return Result<ExternalAuthProviderResult>.Failure(
				ExternalAuthErrors.InvalidProviderToken(externalProvider: EExternalAuthProvider.Google));
		}

		catch (HttpRequestException exception) 
		{
			throw new ExternalProviderUnavailableException(provider: EExternalAuthProvider.Google,
														   innerException: exception);
		}

		catch (OperationCanceledException) when (ct.IsCancellationRequested) 
		{
			throw;
		}

		catch (OperationCanceledException exception) 
		{
			throw new ExternalProviderUnavailableException(provider: EExternalAuthProvider.Google,
														   innerException: exception);
		}
	}

}
