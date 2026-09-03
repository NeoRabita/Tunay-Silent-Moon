using SilentMoonApp.SharedKernel.Resources;

namespace SilentMoonApp.Application.Errors;

public static class TrackProgressErrors
{
	public static Error NotFound()
		=> Error.NotFound(code: "trackprogress_not_found",
					  message: ErrorMessages.TrackProgressNotFound);

	public static Error InvalidPosition(int durationSec)
		=> Error.Validation(code: "trackprogress_invalid_position",
							message: ErrorMessages.TrackProgressInvalidPosition,
							details: new 
							{ 
								DurationSec = durationSec 
							});
}
