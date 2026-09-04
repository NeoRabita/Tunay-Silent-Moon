using SilentMoonApp.SharedKernel.Resources;

namespace SilentMoonApp.Application.Errors;

public static class CourseFavoriteErrors
{
	public static Error AlreadyExists()
		=> Error.Conflict(code: "coursefavorite_already_exists",
						  message: ErrorMessages.CourseFavoriteAlreadyExists);

	public static Error NotFound()
		=> Error.NotFound(code: "coursefavorite_not_found",
						  message: ErrorMessages.CourseFavoriteNotFound);
}
