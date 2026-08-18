using SilentMoonApp.Application.Exceptions.Common;

namespace SilentMoonApp.Application.Exceptions.Storage;

public class StorageNotFoundException : AppException
{
	public StorageNotFoundException(string containerName,
						   string storedFileName) : base(code: "storage.not_found",
														 message: $"The file '{storedFileName}' was not found in the storage container '{containerName}'.")	
	{
		ContainerName = containerName;
		StoredFileName = storedFileName;
	}

	public string ContainerName { get; }
	public string StoredFileName { get; }
}
