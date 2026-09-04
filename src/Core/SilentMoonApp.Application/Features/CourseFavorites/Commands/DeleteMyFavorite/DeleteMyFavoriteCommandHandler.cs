using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Errors;
using SilentMoonApp.Application.Messaging;
using SilentMoonApp.Domain.Entities;

namespace SilentMoonApp.Application.Features.CourseFavorites.Commands.DeleteMyFavorite;

public class DeleteMyFavoriteCommandHandler : ICommandHandler<DeleteMyFavoriteCommand>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly ICurrentUser _currentUser;

	public DeleteMyFavoriteCommandHandler(IUnitOfWork unitOfWork,
										  ICurrentUser currentUser)
	{
		_unitOfWork = unitOfWork;
		_currentUser = currentUser;
	}


	public async Task<Result<NoResult>> Handle(DeleteMyFavoriteCommand command,
										 CancellationToken ct = default)
	{
		if (!_currentUser.IsAuthenticated || _currentUser.UserId is not Guid userId)
			return Result<NoResult>.Failure(
				AuthErrors.UnAuthorized());

		CourseFavorite? courseFavorite = await _unitOfWork.ReadRepository<CourseFavorite>()
														  .GetAsync(filter: cf => cf.CourseId == command.CourseId
																			   && cf.UserId == userId,
																	tracking: true,
																	cancellationToken: ct);
		if (courseFavorite is null)
			return Result<NoResult>.Failure(
				CourseFavoriteErrors.NotFound());

		_unitOfWork.WriteRepository<CourseFavorite>()
				   .Remove(courseFavorite);


		return Result<NoResult>.Success(NoResult.Value);
	}
}
