using SilentMoonApp.Application.Abstractions.Repositories.Write;
using SilentMoonApp.Domain.Entities;
using SilentMoonApp.Infrastructure.Persistence.Contexts;

namespace SilentMoonApp.Infrastructure.Persistence.Repositories.Write;

public class TopicWriteRepository : WriteRepository<Topic>, ITopicWriteRepository
{
	public TopicWriteRepository(AppDbContext dbContext) : base(dbContext) { }
}
