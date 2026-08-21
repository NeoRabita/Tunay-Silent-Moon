using SilentMoonApp.Application.Abstractions.Messaging;


namespace SilentMoonApp.Application.Features.Topics.Commands.UpdateMyTopics;

public sealed record UpdateMyTopicsCommand
(
	IReadOnlyList<Guid> TopicIds
):ICommand<IReadOnlyList<UpdateMyTopicsResult>>;
