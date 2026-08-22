using SilentMoonApp.Domain.Entities.Identity;

namespace SilentMoonApp.Application.Abstractions.Repositories.Read;

public interface IUserReadRepository : IReadRepository<User> 
{
	Task<User?> GetByEmailAsync(string email,
							    bool tracking = false,
							    CancellationToken cancellationToken = default);

	Task<User?> GetByIdWithTopicsAsync(Guid userId,
									   bool tracking = false,
									   CancellationToken cancellationToken = default);

	Task<User?> GetByIdWithRemindersAsync(Guid userId,
										  bool tracking = false,
										  CancellationToken cancellationToken = default);
}
