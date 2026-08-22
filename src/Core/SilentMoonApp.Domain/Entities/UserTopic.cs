using SilentMoonApp.Domain.Entities.Identity;

namespace SilentMoonApp.Domain.Entities;

public class UserTopic:BaseEntity
{
	// Properties
	public Guid UserId { get; set; }
	public Guid TopicId { get; set; }


	// Relations
	public User User { get; set; } = null!;
	public Topic Topic { get; set; } = null!;
}
