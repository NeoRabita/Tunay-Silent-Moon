using SilentMoonApp.Domain.Entities;
using System.Linq.Expressions;


namespace SilentMoonApp.Application.Abstractions.Repositories.Read;

public interface ICourseReadRepository : IReadRepository<Course>
{
	IQueryable<Course> QueryForList(Expression<Func<Course, bool>> filter,
									bool tracking = false);

	IQueryable<Course> QueryForDetails(Expression<Func<Course, bool>> filter,
									   bool tracking = false);

	Task<Course?> GetCourseDetailAsync(Guid id,
									   bool tracking = false,
									   CancellationToken cancellationToken = default);
}
