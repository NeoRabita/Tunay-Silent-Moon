using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.Errors;


namespace SilentMoonApp.Application.Features.Topics.Queries.GetMyTopics;

public class GetMyTopicsQueryHandler : IQueryHandler<GetMyTopicsQuery, IReadOnlyList<GetMyTopicsResult>>
{
	private readonly ICurrentUser _currentUser;
	private readonly IUnitOfWork _unitOfWork;

	public GetMyTopicsQueryHandler(ICurrentUser currentUser,
								   IUnitOfWork unitOfWork)
	{
		_currentUser = currentUser;
		_unitOfWork = unitOfWork;
	}


	public async Task<Result<IReadOnlyList<GetMyTopicsResult>>> Handle(GetMyTopicsQuery query,
															     CancellationToken ct = default)
	{
		if(!_currentUser.IsAuthenticated ||
			_currentUser.UserId is not Guid userId)

			return Result<IReadOnlyList<GetMyTopicsResult>>.Failure(
				AuthErrors.UnAuthorized());

		User? user = await _unitOfWork.Repository<IUserReadRepository>()
									  .GetByIdWithTopicsAsync(userId: userId,
															  tracking: false,
															  cancellationToken: ct);

		if(user is null || user.IsDeleted)
			return Result<IReadOnlyList<GetMyTopicsResult>>.Failure(
				AuthErrors.UnAuthorized());


		return Result<IReadOnlyList<GetMyTopicsResult>>.Success(
			user.UserTopics
				.Where(userTopic => !userTopic.Topic.IsDeleted)
				.Select(
					topic => new GetMyTopicsResult
					(
						Id: topic.Topic.Id,
						Slug: topic.Topic.Slug,
						Title: topic.Topic.Title,
						IconUrl: topic.Topic.IconUrl,
						ColorHex: topic.Topic.ColorHex
					)
				
			).ToList()
		);
	}
}
