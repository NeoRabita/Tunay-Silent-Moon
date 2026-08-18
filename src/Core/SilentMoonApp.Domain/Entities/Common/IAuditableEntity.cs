namespace SilentMoonApp.Domain.Entities.Common;

public interface IAuditableEntity
{
	Guid? CreatedBy { get; set; }
	Guid? UpdatedBy { get; set; }

	DateTimeOffset CreatedAt { get; set; }
	DateTimeOffset? UpdatedAt { get; set; }
}
