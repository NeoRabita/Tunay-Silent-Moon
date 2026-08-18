using SilentMoonApp.SharedKernel.Primitives;
using SilentMoonApp.SharedKernel.Resources;

namespace SilentMoonApp.Application.Errors;

public static class StorageErrors
{
	public static Error InvalidFile()

		=> Error.Validation(code: "storage.invalid_file",
							message: ErrorMessages.StorageInvalidFile);


	public static Error InvalidFileName()

		=> Error.Validation(code: "storage.invalid_file_name",
							message: ErrorMessages.StorageInvalidFileName);


	public static Error InvalidContentType()
		=> Error.Validation(code: "storage.invalid_content_type",
							message: string.Format(ErrorMessages.StorageInvalidContentType));


	public static Error FileNotFound()

		=> Error.NotFound(code: "storage.file_not_found",
						  message: ErrorMessages.StorageFileNotFound);


	public static Error FileTooLarge(long maxSizeBytes)

		=> Error.Validation(code: "storage.invalid_file_type",
							message: string.Format(ErrorMessages.StorageFileTooLarge,
												   maxSizeBytes));


	public static Error UnSupportedFileType()

		=> Error.Validation(code: "storage.unsupported_file_type",
							message: ErrorMessages.StorageUnsupportedFileType);


	public static Error ContentTypeMissMatch()

		=> Error.Conflict(code: "storage.content_type_mismatch",
						  message: ErrorMessages.StorageContentTypeMismatch);

}
