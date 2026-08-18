namespace SilentMoonApp.Domain.Entities.Common;

public interface ISoftDeletableEntity
{
	bool IsDeleted { get; set; }
	Guid? DeletedBy { get; set; }
	DateTimeOffset? DeletedAt { get; set; }
}
