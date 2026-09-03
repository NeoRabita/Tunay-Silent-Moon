using SilentMoonApp.Domain.Entities;


namespace SilentMoonApp.Application.Abstractions.Repositories.Read;

public interface ITrackProgressReadRepository : IReadRepository<TrackProgress>
{
	IQueryable<TrackProgress> QueryMyHistory(Guid userId);
}
