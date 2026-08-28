namespace SilentMoonApp.WebAPI.Contracts.Courses.GetRelatedCourses;

public sealed class GetRelatedCoursesRequest
{
	public int Limit { get; set; } = 20;
}
