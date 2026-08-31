namespace SilentMoonApp.Application.Features.Home.Queries.GetHome;

public sealed record GetHomeResult
(
	GetHomeGreetingResult Greeting,

	GetHomeSectionResult Recommended,
	GetHomeSectionResult? DailyThought,
	GetHomeSectionResult FeaturedSleep,
	GetHomeSectionResult PopularMeditations
);


public sealed record GetHomeGreetingResult
(
	string Title,
	string Message
);


public sealed record GetHomeSectionResult
(
	string Title,
	IReadOnlyList<GetHomeCourseSectionResult> Courses
);


public sealed record GetHomeCourseSectionResult
(
	Guid Id,
	string Title,
	string SubTitle,
	string CategoryType,
	Guid CategoryId,
	string ImageUrl,
	int DurationSec,
	bool IsFeatured,
	IReadOnlyList<string> Narrators
);
