using SilentMoonApp.Domain.Enums;
using SilentMoonApp.Domain.Entities.Common;


namespace SilentMoonApp.Domain.Entities.Files;

public abstract class FileBase : BaseEntity,
								 IAuditableEntity,
								 ISoftDeletableEntity
{
	// Properties
	public string ContainerName { get; set; } = string.Empty;
	public string StoredFileName { get; set; } = string.Empty;
	public string UploadedFileName { get; set; } = string.Empty;
	public string Extension { get; set; } = string.Empty;

	public long SizeBytes { get; set; }
	public string ContentType { get; set; } = string.Empty;

	public EStorageProvider StorageProvider { get; set; } = EStorageProvider.Local;


	// Auditable Properties
	public Guid? CreatedBy { get; set; }
	public Guid? UpdatedBy { get; set; }

	public DateTimeOffset CreatedAt { get; set; } 
	public DateTimeOffset? UpdatedAt { get; set; }


	// Soft Deletable Properties
	public bool IsDeleted { get; set; }
	public Guid? DeletedBy { get; set; }
	public DateTimeOffset? DeletedAt { get; set; }
}
