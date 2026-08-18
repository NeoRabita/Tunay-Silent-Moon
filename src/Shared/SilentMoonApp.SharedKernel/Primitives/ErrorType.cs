namespace SilentMoonApp.SharedKernel.Primitives;

public enum ErrorType
{
	None = 0,

	Validation = 1,

	NotFound = 2,

	Conflict = 3,

	UnAuthorized = 4,

	Forbidden = 5,

	BusinessRule = 6,

	UnAvailable = 7,

	ExternalProvider = 8,

	TooManyRequests = 9,

	Locked = 10
}
