using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using SilentMoonApp.Application.Abstractions.Messaging.Execution;
using SilentMoonApp.Application.Features.Home.Queries.GetHome;
using SilentMoonApp.SharedKernel.Primitives;
using SilentMoonApp.WebAPI.Contracts.Home;

namespace SilentMoonApp.WebAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/home")]
public class HomeController : BaseController
{
	private readonly IDispatcher _dispatcher;

	public HomeController(IDispatcher dispatcher)
	{
		_dispatcher = dispatcher;
	}

	[HttpGet]
	[ProducesResponseType(typeof(GetHomeResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> GetHome(CancellationToken cancellationToken)
	{
		Result<GetHomeResult> result = await _dispatcher.SendAsync(
			query: new GetHomeQuery(),
			cancellationToken: cancellationToken);

		return HandleResult(
			result: result,
			onSuccess: home => Ok(new GetHomeResponse
			{
				Greeting = new GetHomeGreetingResponse
				{
					Title = home.Greeting.Title,
					Message = home.Greeting.Message
				},

				Recommended = new GetHomeSectionResponse
				{
					Title = home.Recommended.Title,
					Items = home.Recommended.Courses.Select(course => new GetHomeCourseItemResponse
					{
						Id = course.Id,
						Title = course.Title,
						Subtitle = course.SubTitle,
						Type = course.CategoryType,
						CategoryId = course.CategoryId,
						ImageUrl = course.ImageUrl,
						DurationSec = course.DurationSec,
						IsFeatured = course.IsFeatured,
						Narrators = course.Narrators
					}).ToList()
				},

				DailyThought = new GetHomeSectionResponse
				{
					Title = home.DailyThought!.Title,
					Items = home.DailyThought.Courses.Select(course => new GetHomeCourseItemResponse
					{
						Id = course.Id,
						Title = course.Title,
						Subtitle = course.SubTitle,
						Type = course.CategoryType,
						CategoryId = course.CategoryId,
						ImageUrl = course.ImageUrl,
						DurationSec = course.DurationSec,
						IsFeatured = course.IsFeatured,
						Narrators = course.Narrators
					}).ToList()
				},

				FeaturedSleep = new GetHomeSectionResponse
				{
					Title = home.FeaturedSleep.Title,
					Items = home.FeaturedSleep.Courses.Select(course => new GetHomeCourseItemResponse
					{
						Id = course.Id,
						Title = course.Title,
						Subtitle = course.SubTitle,
						Type = course.CategoryType,
						CategoryId = course.CategoryId,
						ImageUrl = course.ImageUrl,
						DurationSec = course.DurationSec,
						IsFeatured = course.IsFeatured,
						Narrators = course.Narrators
					}).ToList()
				},

				PopularMeditations = new GetHomeSectionResponse
				{
					Title = home.PopularMeditations.Title,
					Items = home.PopularMeditations.Courses.Select(course => new GetHomeCourseItemResponse
					{
						Id = course.Id,
						Title = course.Title,
						Subtitle = course.SubTitle,
						Type = course.CategoryType,
						CategoryId = course.CategoryId,
						ImageUrl = course.ImageUrl,
						DurationSec = course.DurationSec,
						IsFeatured = course.IsFeatured,
						Narrators = course.Narrators
					}).ToList()
				}
			})
		);
	}
}
