using SilentMoonApp.Application.DTOs.Storage;
using SilentMoonApp.Domain.Entities.Identity;


namespace SilentMoonApp.Application.Abstractions.Storage;

public interface IUserAvatarService
{
	Task<string> GetAvatarUrlAsync(Guid? avatarImageFileId,
								   CancellationToken cancellationToken = default);

	Task<Result<string>> UploadAndAssignAvatarAsync(User user,
													StorageUploadRequest avatarFile,
													Guid actorUserId,
													DateTimeOffset nowUtc,
													CancellationToken cancellationToken = default);
}
