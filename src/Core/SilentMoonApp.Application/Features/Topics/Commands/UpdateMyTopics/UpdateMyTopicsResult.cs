using SilentMoonApp.Application.Features.Topics.Queries.GetTopics;

namespace SilentMoonApp.Application.Features.Topics.Commands.UpdateMyTopics;

public sealed record UpdateMyTopicsResult(Guid Id,
										  string Slug,
										  string Title,
										  string IconUrl,
										  string ColorHex):GetTopicsResult(Id: Id,
																		   Slug: Slug,
																		   Title: Title,
																		   IconUrl: IconUrl,
																		   ColorHex: ColorHex);