using SilentMoonApp.Domain.Entities;
using SilentMoonApp.Infrastructure.Persistence.Contexts;
using SilentMoonApp.Application.Abstractions.Repositories.Read;


namespace SilentMoonApp.Infrastructure.Persistence.Repositories.Read;

public class TopicReadRepository : ReadRepository<Topic>, ITopicReadRepository
{
	public TopicReadRepository(AppDbContext dbContext) : base(dbContext) { }
}
