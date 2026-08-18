using Microsoft.AspNetCore.Mvc;
using SilentMoonApp.SharedKernel.Primitives;
using Microsoft.AspNetCore.Authorization;
using SilentMoonApp.WebAPI.Contracts.Profile.GetMyProfile;
using SilentMoonApp.WebAPI.Contracts.Profile.UpdateMyProfile;
using SilentMoonApp.Application.Abstractions.Messaging.Execution;
using SilentMoonApp.Application.Features.Profile.Queries.GetMyProfile;
using SilentMoonApp.Application.Features.Profile.Commands.UpdateMyProfile;
using SilentMoonApp.Application.DTOs.Storage;


namespace SilentMoonApp.WebAPI.Controllers;

[Authorize]
[ApiController]

[Route("api/profile")]

public class ProfileController : BaseController
{
	private readonly IDispatcher _dispatcher;

	public ProfileController(IDispatcher dispatcher)
	{
		_dispatcher = dispatcher;
	}




	[HttpGet("me")]

	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]

	public async Task<IActionResult> GetMyProfile(CancellationToken ct)
	{
		GetMyProfileQuery query = new();

		Result<GetMyProfileResult> result = await _dispatcher.SendAsync(query: query,
																cancellationToken: ct);

		return HandleResult(
			result: result,

			onSuccess: profile =>
				Ok(new GetMyProfileResponse
				{
					Id = profile.Id,
					Name = profile.Name,
					Email = profile.Email,
					IsEmailVerified = profile.IsEmailVerified,
					AvatarUrl = profile.AvatarUrl,
					CreatedAt = profile.CreatedAt
				}
			)
		);
	}



	[HttpPatch("me")]

	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]

	public async Task<IActionResult> UpdateMyProfile([FromForm] UpdateMyProfileRequest request,
																CancellationToken ct)
	{
		StorageUploadRequest? avatarFile = null;


		if (request.AvatarImage is not null)

			avatarFile = new StorageUploadRequest
			(
				FileStream : request.AvatarImage.OpenReadStream(),
				UploadedFileName : request.AvatarImage.FileName,
				ContentType : request.AvatarImage.ContentType,
				SizeBytes : request.AvatarImage.Length, 

				ContainerName : "images",
				DirectoryPath : "avatars"
			);


		UpdateMyProfileCommand command = new(Name: request.Name,
											 AvatarFile: avatarFile);

		Result<UpdateMyProfileResult> result = await _dispatcher.SendAsync(command: command,
																	cancellationToken: ct);

		return HandleResult(
			result: result,

			onSuccess: profile =>
				Ok(new UpdateMyProfileResponse
				{
					Id = profile.Id,
					Name = profile.Name,
					Email = profile.Email,
					IsEmailVerified = profile.IsEmailVerified,
					AvatarUrl = profile.AvatarUrl,
					CreatedAt = profile.CreatedAt
				}
			)
		);
	}

}
