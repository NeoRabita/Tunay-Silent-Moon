using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SilentMoonApp.Domain.Entities;
using SilentMoonApp.Infrastructure.Persistence.Contexts;
using SilentMoonApp.Application.Abstractions.Repositories.Read;


namespace SilentMoonApp.Infrastructure.Persistence.Repositories.Read;

public class CourseReadRepository : ReadRepository<Course>, ICourseReadRepository
{
	public CourseReadRepository(AppDbContext dbContext) : base(dbContext) { }


	public IQueryable<Course> QueryForDetails(Expression<Func<Course, bool>> filter,
											  bool tracking = false)
		=> Query(filter: filter,
				 includes: course => course.Include(course => course.Category)
											   .ThenInclude(category => category.CategoryType)
										   .Include(course => course.CoverImageFile)
										   .Include(course => course.Tracks)
											   .ThenInclude(track => track.Narrator)
										   .Include(course => course.Tracks)
											   .ThenInclude(track => track.AudioFile)
										   .Include(course => course.Tracks)
											   .ThenInclude(track => track.CoverImageFile),
				 tracking: tracking);


	public IQueryable<Course> QueryForList(Expression<Func<Course, bool>> filter,
										   bool tracking = false)
		=> Query(filter: filter,
				 includes: course => course.Include(course => course.Category)
				 							 	.ThenInclude(category => category.CategoryType)
				 							 .Include(course => course.CoverImageFile)
				 							 .Include(course => course.Tracks)
				 							 	.ThenInclude(track => track.Narrator)
				 							 .Include(course => course.Tracks)
				 							 	.ThenInclude(track => track.AudioFile)
				 							 .Include(course => course.Tracks)
				 							 	.ThenInclude(track => track.CoverImageFile),
				 tracking: tracking);


	public async Task<Course?> GetCourseDetailAsync(Guid id,
													bool tracking = false,
													CancellationToken cancellationToken = default)
		=> await QueryForDetails(filter: course => course.Id == id
												&& !course.IsDeleted
												&& !course.Category.IsDeleted
												&& !course.Category.CategoryType.IsDeleted,
								 tracking: tracking)
				.FirstOrDefaultAsync(cancellationToken: cancellationToken);
}
