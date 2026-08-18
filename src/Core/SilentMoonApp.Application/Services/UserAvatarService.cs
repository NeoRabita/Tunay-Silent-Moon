using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Abstractions.Repositories.Write;
using SilentMoonApp.Application.Abstractions.Storage;
using SilentMoonApp.Application.DTOs.Storage;
using SilentMoonApp.Domain.Entities.Files;


namespace SilentMoonApp.Application.Services;

public sealed class UserAvatarService : IUserAvatarService
{
	private readonly IStorageService _storageService;
	private readonly IUnitOfWork _unitOfWork;

	public UserAvatarService(IStorageService storageService,
							 IUnitOfWork unitOfWork)
	{
		_storageService = storageService;
		_unitOfWork = unitOfWork;
	}



	public async Task<string> GetAvatarUrlAsync(Guid? avatarImageFileId,
												CancellationToken cancellationToken = default)
	{
		if (avatarImageFileId is not Guid imageFileId)
			return string.Empty;

		
		ImageFile? avatarImageFile = await _unitOfWork.ReadRepository<ImageFile>().GetByIdAsync(id: imageFileId,
																								tracking: false,
																								cancellationToken: cancellationToken);
		
		if (avatarImageFile is null || avatarImageFile.IsDeleted)
			return string.Empty;


		Result<string> urlResult = await _storageService.GetFileUrlAsync(fileReference: GenerateFileReference(avatarImageFile),
																		 cancellationToken: cancellationToken);

		return urlResult.IsSuccess
			? urlResult.Value
			: string.Empty;
	}


	public async Task<Result<string>> UploadAndAssignAvatarAsync(User user,
																 StorageUploadRequest avatarFile,
																 Guid actorUserId,
																 DateTimeOffset nowUtc,
																 CancellationToken cancellationToken = default)
	{
		Result<StorageUploadResult> uploadResult = await _storageService.UploadFileAsync(
			request: avatarFile,
			cancellationToken: cancellationToken);

		
		if (uploadResult.IsFailure)
			return Result<string>.Failure(uploadResult.Error);

		StorageUploadResult uploadedFile = uploadResult.Value;


		ImageFile imageFile = new()
		{
			StorageProvider = uploadedFile.Provider,
			ContainerName = uploadedFile.ContainerName,
			StoredFileName = uploadedFile.StoredFileName,
			UploadedFileName = uploadedFile.UploadedFileName,
			Extension = uploadedFile.Extension,
			ContentType = uploadedFile.ContentType,
			SizeBytes = uploadedFile.SizeBytes,
			CreatedAt = nowUtc,
			CreatedBy = actorUserId,
		};


		await _unitOfWork.WriteRepository<ImageFile>().AddAsync(
			entity: imageFile,
			cancellationToken: cancellationToken);


		user.AvatarImageFileId = imageFile.Id;
		user.AvatarImageFile = imageFile;

		
		Result<string> urlResult = await _storageService.GetFileUrlAsync(
			fileReference: GenerateFileReference(imageFile),
			cancellationToken: cancellationToken);


		return urlResult.IsSuccess
			? Result<string>.Success(urlResult.Value)
			: Result<string>.Failure(urlResult.Error);
	}


	private static StorageFileReference GenerateFileReference(ImageFile imageFile)
		
		=> new(StorageProvider: imageFile.StorageProvider,
		   	   ContainerName: imageFile.ContainerName,
		   	   StoredFileName: imageFile.StoredFileName);
}
