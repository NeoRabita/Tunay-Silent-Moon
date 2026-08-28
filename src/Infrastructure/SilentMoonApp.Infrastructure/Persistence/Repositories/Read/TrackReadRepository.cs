using Microsoft.EntityFrameworkCore;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Domain.Entities;
using SilentMoonApp.Infrastructure.Persistence.Contexts;

namespace SilentMoonApp.Infrastructure.Persistence.Repositories.Read;

public class TrackReadRepository : ReadRepository<Track>, ITrackReadRepository
{
	public TrackReadRepository(AppDbContext dbContext) : base(dbContext) { }

	public Task<Track?> GetTrackDetailAsync(Guid id,
											bool tracking = false,
											CancellationToken ct = default)

		=> GetAsync(filter: track => track.Id == id
								  && !track.IsDeleted
								  && !track.Course.IsDeleted
								  && !track.Narrator.IsDeleted
								  && track.AudioFile != null
								  && !track.AudioFile.IsDeleted,

					includes: query => query.Include(track => track.Course)
												  .ThenInclude(course => course.CoverImageFile)
											.Include(track => track.Narrator)
											.Include(track => track.AudioFile)
											.Include(track => track.CoverImageFile),

					tracking: tracking,
					ct: ct);
}
