using SilentMoonApp.SharedKernel.Resources;

namespace SilentMoonApp.Application.Errors;

public static class CourseErrors
{
	public static Error NotFound()
		=> Error.NotFound(code: "course_not_found",
						  message: ErrorMessages.CourseNotFound);
}
