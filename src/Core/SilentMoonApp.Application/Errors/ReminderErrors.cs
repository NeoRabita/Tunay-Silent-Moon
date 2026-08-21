using SilentMoonApp.SharedKernel.Resources;

namespace SilentMoonApp.Application.Errors;

public static class ReminderErrors
{
	public static Error NotFound()
		=> Error.NotFound(code: "reminder_not_found",
						  message: ErrorMessages.ReminderNotFound);
}
