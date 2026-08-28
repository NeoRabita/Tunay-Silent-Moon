using SilentMoonApp.SharedKernel.Resources;

namespace SilentMoonApp.Application.Errors;

public static class TrackErrors
{
	public static Error NotFound()
		=> Error.NotFound(code: "track_not_found",
						  message: ErrorMessages.TrackNotFound);
}
