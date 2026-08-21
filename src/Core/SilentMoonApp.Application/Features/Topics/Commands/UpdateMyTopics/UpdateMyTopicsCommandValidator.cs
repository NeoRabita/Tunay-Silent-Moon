using FluentValidation;

namespace SilentMoonApp.Application.Features.Topics.Commands.UpdateMyTopics;

public sealed class UpdateMyTopicsCommandValidator:AbstractValidator<UpdateMyTopicsCommand>
{
	public UpdateMyTopicsCommandValidator()
	{
		RuleFor(command => command.TopicIds)
			.NotNull()
			.WithMessage("TopicIds tələb olunur.");


		RuleForEach(command => command.TopicIds)
			.NotEqual(Guid.Empty)
			.WithMessage("Topic id boş ola bilməz.");


		RuleFor(command => command.TopicIds)
			.Must(topicIds => topicIds.Distinct().Count() == topicIds.Count)
			.WithMessage("Eyni topic bir neçə dəfə göndərilə bilməz.");
	}
}