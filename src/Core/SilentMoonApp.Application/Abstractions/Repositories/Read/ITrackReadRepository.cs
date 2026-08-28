using SilentMoonApp.Domain.Entities;

namespace SilentMoonApp.Application.Abstractions.Repositories.Read;

public interface ITrackReadRepository : IReadRepository<Track>
{
	Task<Track?> GetTrackDetailAsync(Guid id,
									 bool tracking = false,
									 CancellationToken cancellationToken = default);
}
