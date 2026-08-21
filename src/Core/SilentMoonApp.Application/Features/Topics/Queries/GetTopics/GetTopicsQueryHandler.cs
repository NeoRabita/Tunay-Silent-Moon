using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.Features.Auth.Commands.Refresh;
using SilentMoonApp.Domain.Entities;

namespace SilentMoonApp.Application.Features.Topics.Queries.GetTopics;

public class GetTopicsQueryHandler : IQueryHandler<GetTopicsQuery, IReadOnlyList<GetTopicsResult>>
{
	private readonly IUnitOfWork _unitOfWork;

	public GetTopicsQueryHandler(IUnitOfWork unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}


	public async Task<Result<IReadOnlyList<GetTopicsResult>>> Handle(GetTopicsQuery request, CancellationToken ct = default)
	{
		IReadOnlyList<Topic> topics = await _unitOfWork.ReadRepository<Topic>().GetAllAsync(filter: topic => topic.IsDeleted == false,
																						    cancellationToken: ct);

		return Result<IReadOnlyList<GetTopicsResult>>.Success(
			topics.Select(topic => 
				new GetTopicsResult
				(
					Id : topic.Id,
					Slug : topic.Slug,
					Title : topic.Title,
					IconUrl : topic.IconUrl,
					ColorHex : topic.ColorHex
				)
			).ToList()
		);
	}
}
