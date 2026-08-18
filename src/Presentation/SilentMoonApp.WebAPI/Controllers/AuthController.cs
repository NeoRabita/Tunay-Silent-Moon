using Microsoft.AspNetCore.Authorization;
using SilentMoonApp.Application.Abstractions.Messaging.Execution;
using Microsoft.AspNetCore.RateLimiting;
using SilentMoonApp.Application.Features.Auth.Commands.FacebookAuth;
using SilentMoonApp.Application.Features.Auth.Commands.ForgotPassword;
using SilentMoonApp.Application.Features.Auth.Commands.GoogleAuth;
using SilentMoonApp.Application.Features.Auth.Commands.Login;
using SilentMoonApp.Application.Features.Auth.Commands.Logout;
using SilentMoonApp.Application.Features.Auth.Commands.Refresh;
using SilentMoonApp.Application.Features.Auth.Commands.Register;
using SilentMoonApp.Application.Features.Auth.Commands.ResendEmailOtp;
using SilentMoonApp.Application.Features.Auth.Commands.ResetPassword;
using SilentMoonApp.Application.Features.Auth.Commands.VerifyEmail;
using SilentMoonApp.Application.Messaging;
using SilentMoonApp.SharedKernel.Primitives;
using SilentMoonApp.WebAPI.Contracts.Auth.FacebookAuth;
using SilentMoonApp.WebAPI.Contracts.Auth.ForgotPassword;
using SilentMoonApp.WebAPI.Contracts.Auth.GoogleAuth;
using SilentMoonApp.WebAPI.Contracts.Auth.Login;
using SilentMoonApp.WebAPI.Contracts.Auth.Logout;
using SilentMoonApp.WebAPI.Contracts.Auth.Refresh;
using SilentMoonApp.WebAPI.Contracts.Auth.Register;
using SilentMoonApp.WebAPI.Contracts.Auth.ResendEmailOtp;
using SilentMoonApp.WebAPI.Contracts.Auth.ResetPassword;
using SilentMoonApp.WebAPI.Contracts.Auth.VerifyEmail;
using SilentMoonApp.WebAPI.HttpContexts;
using System.Diagnostics;


namespace SilentMoonApp.WebAPI.Controllers;


[Route("api/auth")]
public class AuthController : BaseController
{
	private readonly IDispatcher _dispatcher;
	private readonly HttpCookieService _httpCookieService;
	private readonly ILogger<AuthController> _logger;

	public AuthController(IDispatcher dispatcher,
						  HttpCookieService httpCookieService,
						  ILogger<AuthController> logger)
	{
		_dispatcher = dispatcher;
		_httpCookieService = httpCookieService;
		_logger = logger;
	}




	[AllowAnonymous]
	[HttpPost("register")]
	[EnableRateLimiting("auth-otp")]

	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status409Conflict)]
	[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
	public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest,
														 CancellationToken cancellationToken)
	{
		RegisterCommand registerCommand = new RegisterCommand
		(
			FirstName: registerRequest.FirstName,
			LastName: registerRequest.LastName,
			UserName: registerRequest.UserName,
			Email: registerRequest.Email,
			Password: registerRequest.Password
		);


		Result<RegisterResult> result = await _dispatcher.SendAsync(registerCommand, cancellationToken);


		return HandleResult(
			result,
			registerResult => Ok(new RegisterResponse
			{
				Message = registerResult.Message,
				Email = registerResult.Email,
				OtpExpiresAt = registerResult.OtpExpiresAt
			})
		);
	}



	[AllowAnonymous]
	[HttpPost("login")]
	[EnableRateLimiting("auth-login")]

	[ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status423Locked)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]

	public async Task<IActionResult> Login(LoginRequest loginRequest,
										   CancellationToken cancellationToken)
	{
		LoginCommand loginCommand = new LoginCommand(Email: loginRequest.Email,
													 Password: loginRequest.Password);

		Result<LoginResult> result = await _dispatcher.SendAsync(loginCommand, cancellationToken);


		return HandleResult(
			result,
			loginResult =>
			{

				_httpCookieService.Set(response: Response,
									   value: result.Value.RawRefreshToken,
									   expiresAt: result.Value.RefreshTokenExpiresAt);


				return Ok(new LoginResponse
				{
					AccessToken = result.Value.AccessToken,
					TokenType = result.Value.TokenType,
					AccessTokenExpiresIn = result.Value.AccessTokenExpiresIn,

					User = new LoginUserResponse
					{
						Id = result.Value.User.Id,
						FirstName = result.Value.User.FirstName,
						Email = result.Value.User.Email,
						IsEmailVerified = result.Value.User.IsEmailVerified,
						AvatarUrl = result.Value.User.AvatarUrl!,
						CreatedAt = result.Value.User.CreatedAt
					}
				});
			}
		);
	}



	[AllowAnonymous]
	[HttpPost("verify-email")]
	[EnableRateLimiting("auth-otp")]

	[ProducesResponseType(typeof(VerifyEmailResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status410Gone)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]

	public async Task<IActionResult> VerifyEmail(VerifyEmailRequest request,
												 CancellationToken cancellationToken)
	{
		VerifyEmailCommand command = new VerifyEmailCommand(Email: request.Email,
															OtpCode: request.OtpCode);

		Result<VerifyEmailResult> result = await _dispatcher.SendAsync(command: command,
																		  cancellationToken: cancellationToken);

		return HandleResult(
			result,
			verifyResult =>
			{

				_httpCookieService.Set(
					response: Response,
					value: verifyResult.RefreshToken,
					expiresAt: verifyResult.RefreshTokenExpiresAt);


				return Ok(new VerifyEmailResponse
				{
					AccessToken = verifyResult.AccessToken,
					TokenType = verifyResult.TokenType,
					AccessTokenExpiresIn = verifyResult.AccessTokenExpiresIn,

					User = new VerifyEmailUserResponse
					{
						Id = verifyResult.User.Id,
						FirstName = verifyResult.User.FirstName,
						Email = verifyResult.User.Email,
						IsEmailVerified = verifyResult.User.IsEmailVerified,
						AvatarUrl = verifyResult.User.AvatarUrl,
						CreatedAt = verifyResult.User.CreatedAt
					}
				});
			}
		);

	}



	[AllowAnonymous]
	[HttpPost("resend-otp")]
	[EnableRateLimiting("auth-otp")]

	[ProducesResponseType(typeof(ResendEmailOtpResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]

	public async Task<IActionResult> ResendEmailOtp(ResendEmailOtpRequest request,
																		 CancellationToken cancellationToken)
	{
		ResendEmailOtpCommand command = new(request.Email);

		Result<ResendEmailOtpResult> result = await _dispatcher.SendAsync(command: command,
																  cancellationToken: cancellationToken);

		return HandleResult(
			result,
			resendResult =>
			{
				return Ok(new ResendEmailOtpResponse
				{
					Message = resendResult.Message,
					OtpExpiresAt = resendResult.OtpExpiresAt,
				});
			}
		);
	}



	[AllowAnonymous]
	[HttpPost("refresh")]

	[ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]

	public async Task<IActionResult> RefreshToken(RefreshRequest request,
																  CancellationToken cancellationToken)
	{
		string? rawRefreshToken = !string.IsNullOrWhiteSpace(request?.RefreshToken)
								? request.RefreshToken.Trim()
								: null;


		if (rawRefreshToken is null)
			_httpCookieService.TryGet(request: Request,
									  value: out rawRefreshToken);


		RefreshCommand command = new(RefreshToken: rawRefreshToken!);


		Result<RefreshResult> result = await _dispatcher.SendAsync(command: command,
																   cancellationToken: cancellationToken);

		return HandleResult(
			result,
			refreshResult =>
			{

				_httpCookieService.Set(
					response: Response,
					value: refreshResult.RefreshToken,
					expiresAt: refreshResult.RefreshTokenExpiresAt);


				return Ok(new RefreshResponse
				{
					AccessToken = refreshResult.AccessToken,
					TokenType = refreshResult.TokenType,
					AccessTokenExpiresIn = refreshResult.AccessTokenExpiresIn,

					User = new RefreshUserResponse
					{
						Id = refreshResult.User.Id,
						FirstName = refreshResult.User.FirstName,
						Email = refreshResult.User.Email,
						IsEmailVerified = refreshResult.User.IsEmailVerified,
						AvatarUrl = refreshResult.User.AvatarUrl,
						CreatedAt = refreshResult.User.CreatedAt
					}
				});
			});
	}


	[AllowAnonymous]
	[HttpPost("logout")]

	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]

	public async Task<IActionResult> Logout(LogoutRequest? request,
										CancellationToken cancellationToken)
	{
		string? rawRefreshToken = !string.IsNullOrWhiteSpace(request?.RefreshToken)
								? request.RefreshToken.Trim()
								: null;


		_httpCookieService.TryGet(request: Request,
								  value: out string? cookieRefreshToken);

		string? selectedRefreshToken = rawRefreshToken
									 ?? cookieRefreshToken;


		LogoutCommand command = new(RefreshToken: selectedRefreshToken!);


		Result<NoResult> result = await _dispatcher.SendAsync(command: command,
																 cancellationToken: cancellationToken);


		return HandleResult(
			result,
			_ =>
			{
				bool tokenWasReadFromCookie = rawRefreshToken is null;


				bool bodyTokenMatchesCookie = rawRefreshToken is not null &&
											  cookieRefreshToken is not null &&
											  string.Equals(rawRefreshToken,
															cookieRefreshToken,
											  				StringComparison.Ordinal);


				if (tokenWasReadFromCookie || bodyTokenMatchesCookie)
					_httpCookieService.Delete(Response);


				return NoContent();
			}
		);
	}



	[AllowAnonymous]
	[HttpPost("oauth/google")]
	[EnableRateLimiting("auth-oauth")]

	[ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]

	public async Task<IActionResult> GoogleAuthenticate(GoogleAuthRequest request,
														CancellationToken cancellationToken)
	{
		GoogleAuthCommand command = new GoogleAuthCommand(IdToken: request.idToken);


		Result<GoogleAuthResult> result = await _dispatcher.SendAsync(command: command,
															  cancellationToken: cancellationToken);

		return HandleResult(
			result,
			googleResult =>
			{
				_httpCookieService.Set(
					response: Response,
					value: googleResult.RefreshToken,
					expiresAt: googleResult.RefreshTokenExpiresAt);


				return Ok(new GoogleAuthResponse
				{
					AccessToken = googleResult.AccessToken,
					TokenType = googleResult.TokenType,
					AccessTokenExpiresIn = googleResult.AccessTokenExpiresIn,

					User = new GoogleAuthUserResponse
					{
						Id = googleResult.User.Id,
						FirstName = googleResult.User.Name,
						Email = googleResult.User.Email,
						IsEmailVerified = googleResult.User.EmailVerified,
						AvatarUrl = googleResult.User.AvatarUrl ?? string.Empty,
						CreatedAt = googleResult.User.CreatedAt
					}
				});
			}
		);

	}



	[AllowAnonymous]
	[HttpPost("oauth/facebook")]
	[EnableRateLimiting("auth-oauth")]

	[ProducesResponseType(typeof(ForgotPasswordResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]

	public async Task<IActionResult> FacebookAuthenticate(FacebookAuthRequest request,
														  CancellationToken cancellationToken)
	{
		FacebookAuthCommand command = new FacebookAuthCommand(IdToken: request.idToken);


		Result<FacebookAuthResult> result = await _dispatcher.SendAsync(command: command,
																cancellationToken: cancellationToken);

		return HandleResult(
			result,
			facebookResult =>
			{
				_httpCookieService.Set(
					response: Response,
					value: facebookResult.RefreshToken,
					expiresAt: facebookResult.RefreshTokenExpiresAt);


				return Ok(new FacebookAuthResponse
				{
					AccessToken = facebookResult.AccessToken,
					TokenType = facebookResult.TokenType,
					AccessTokenExpiresIn = facebookResult.AccessTokenExpiresIn,

					User = new FacebookAuthUserResponse
					{
						Id = facebookResult.User.Id,
						FirstName = facebookResult.User.Name,
						Email = facebookResult.User.Email,
						IsEmailVerified = facebookResult.User.EmailVerified,
						AvatarUrl = facebookResult.User.AvatarUrl ?? string.Empty,
						CreatedAt = facebookResult.User.CreatedAt
					}
				});
			}
		);
	}



	[AllowAnonymous]
	[HttpPost("forgot-password")]
	[EnableRateLimiting("auth-otp")]

	[ProducesResponseType(typeof(LoginResponse), StatusCodes.Status202Accepted)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]

	public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request,
													CancellationToken cancellationToken)
	{

		ForgotPasswordCommand command = new ForgotPasswordCommand(Email: request.Email);


		Result<ForgotPasswordResult> result = await _dispatcher.SendAsync(command: command,
																          cancellationToken: cancellationToken);

		return HandleResult(
			result,
			forgotResult =>
				Accepted(new ForgotPasswordResponse
				{
					Message = forgotResult.Message,
					Email = request.Email,
					OtpExpiresAt = forgotResult.OtpExpiresAt
				}
			)
		);

	}



	[AllowAnonymous]
	[HttpPost("reset-password")]
	[EnableRateLimiting("auth-otp")]

	[ProducesResponseType(typeof(ResetPasswordResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]

	public async Task<IActionResult> ResetPassword(ResetPasswordRequest request,
													CancellationToken cancellationToken)
	{

		ResetPasswordCommand command = new ResetPasswordCommand(Email: request.Email,
																OtpCode: request.Otp,
																NewPassword: request.NewPassword,
																ConfirmPassword: request.ConfirmPassword);

		Result<ResetPasswordResult> result = await _dispatcher.SendAsync(command: command,
																		 cancellationToken: cancellationToken);

		return HandleResult(
			result,
			resetResult =>
				Ok(new ResetPasswordResponse
				{
					Message = resetResult.Message
				})
		);
	}

}
