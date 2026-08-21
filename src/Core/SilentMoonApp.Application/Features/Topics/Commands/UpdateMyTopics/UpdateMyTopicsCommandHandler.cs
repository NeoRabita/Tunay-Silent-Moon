using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.Errors;
using SilentMoonApp.Domain.Entities;

namespace SilentMoonApp.Application.Features.Topics.Commands.UpdateMyTopics;

public class UpdateMyTopicsCommandHandler : ICommandHandler<UpdateMyTopicsCommand, IReadOnlyList<UpdateMyTopicsResult>>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly ICurrentUser _currentUser;

	public UpdateMyTopicsCommandHandler(IUnitOfWork unitOfWork,
										ICurrentUser currentUser)
	{
		_unitOfWork = unitOfWork;
		_currentUser = currentUser;
	}


	public async Task<Result<IReadOnlyList<UpdateMyTopicsResult>>> Handle(UpdateMyTopicsCommand command,
																	CancellationToken ct = default)
	{
		if (!_currentUser.IsAuthenticated ||
			_currentUser.UserId is not Guid userId)

			return Result<IReadOnlyList<UpdateMyTopicsResult>>.Failure(
				AuthErrors.UnAuthorized());

		User? user = await _unitOfWork.Repository<IUserReadRepository>()
									  .GetByIdWithTopicsAsync(userId: userId,
															  tracking: true,
															  cancellationToken: ct);

		if (user is null || user.IsDeleted)
			return Result<IReadOnlyList<UpdateMyTopicsResult>>.Failure(
				AuthErrors.UnAuthorized());

		IReadOnlyList<Topic> availableTopics = await _unitOfWork.Repository<ITopicReadRepository>()
																.GetAllAsync(tracking: false,
																			 cancellationToken: ct);

		HashSet<Guid> requestedTopicIds = command.TopicIds.ToHashSet();


		List<Topic> selectedTopics = availableTopics.Where(t => requestedTopicIds.Contains(t.Id))
													.ToList();

		if (selectedTopics.Count != requestedTopicIds.Count)

			return Result<IReadOnlyList<UpdateMyTopicsResult>>.Failure(
				new ValidationError(
					new Dictionary<string, string[]>
					{
						[nameof(command.TopicIds)] =
						["Seçilmiş mövzulardan biri və ya bir neçəsi etibarsızdır."]
					}
				)
			);


		HashSet<Guid> currentTopicIds = user.UserTopics.Select(ut => ut.TopicId)
													   .ToHashSet();


		List<UserTopic> topicsToRemove = user.UserTopics.Where(ut => !requestedTopicIds.Contains(ut.TopicId))
													   .ToList();

		foreach (UserTopic userTopic in topicsToRemove)
		{
			user.UserTopics.Remove(userTopic);
		}

		IEnumerable<Guid> topicsToAdd = requestedTopicIds.Except(currentTopicIds);

		foreach (Guid topicId in topicsToAdd)
			user.UserTopics.Add(new UserTopic
			{
				UserId = userId,
				TopicId = topicId
			});


		return Result<IReadOnlyList<UpdateMyTopicsResult>>.Success(
			selectedTopics.Where(topic => requestedTopicIds.Contains(topic.Id) && !topic.IsDeleted)
						  .Select(topic =>
							   new UpdateMyTopicsResult(Id: topic.Id,
							  							Slug: topic.Slug,
							  							Title: topic.Title,
							  							IconUrl: topic.IconUrl,
							  							ColorHex: topic.ColorHex)
							   )
						  .ToList()
		);
	}
}
