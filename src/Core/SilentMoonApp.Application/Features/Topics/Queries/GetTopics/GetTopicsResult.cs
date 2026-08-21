namespace SilentMoonApp.Application.Features.Topics.Queries.GetTopics;

public record GetTopicsResult
(
	Guid Id,
	string Slug,
	string Title,
	string IconUrl,
	string ColorHex
);
