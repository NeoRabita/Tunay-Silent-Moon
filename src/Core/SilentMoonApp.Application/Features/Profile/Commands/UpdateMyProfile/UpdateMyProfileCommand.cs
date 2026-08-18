using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.DTOs.Storage;


namespace SilentMoonApp.Application.Features.Profile.Commands.UpdateMyProfile;

public sealed record UpdateMyProfileCommand(string Name,
										    StorageUploadRequest? AvatarFile) : ICommand<UpdateMyProfileResult>;
