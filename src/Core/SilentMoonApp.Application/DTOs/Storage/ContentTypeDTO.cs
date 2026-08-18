namespace SilentMoonApp.Application.DTOs.Storage;

public sealed record ContentTypeDTO
(
	string PreferredContentType,
	IReadOnlyCollection<string> AllowedContentTypes
);
