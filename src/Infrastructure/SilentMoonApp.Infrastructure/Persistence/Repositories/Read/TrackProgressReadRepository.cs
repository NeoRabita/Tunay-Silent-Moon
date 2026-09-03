using Microsoft.EntityFrameworkCore;
using Minio.DataModel.Notification;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Domain.Entities;
using SilentMoonApp.Domain.Entities.Identity;
using SilentMoonApp.Infrastructure.Persistence.Contexts;
using System;


namespace SilentMoonApp.Infrastructure.Persistence.Repositories.Read;

public class TrackProgressReadRepository : ReadRepository<TrackProgress>, ITrackProgressReadRepository
{
	public TrackProgressReadRepository(AppDbContext dbContext) : base(dbContext) { }

	public IQueryable<TrackProgress> QueryMyHistory(Guid userId)

		=> Query(filter: progress => progress.UserId == userId
								  && !progress.Track.IsDeleted
								  && !progress.Track.Course.IsDeleted
								  && !progress.Track.Narrator.IsDeleted
								  && progress.Track.AudioFile != null
								  && !progress.Track.AudioFile.IsDeleted,
				 includes: query => query.Include(progress => progress.Track)
				 	 						.ThenInclude(track => track.Course)
												.ThenInclude(course => course.CoverImageFile)
				 						 .Include(progress => progress.Track)
				 	 						.ThenInclude(track => track.Narrator)
				 						 .Include(progress => progress.Track)
				 	 						.ThenInclude(track => track.AudioFile)
				 						 .Include(progress => progress.Track)
				 	 						 .ThenInclude(track => track.CoverImageFile),
				 tracking: false);
}
