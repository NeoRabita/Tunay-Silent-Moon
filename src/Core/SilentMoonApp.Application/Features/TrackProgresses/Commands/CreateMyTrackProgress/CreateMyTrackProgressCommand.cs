using SilentMoonApp.Application.Abstractions.Messaging;


namespace SilentMoonApp.Application.Features.TrackProgresses.Commands.CreateMyTrackProgress;

public sealed record CreateMyTrackProgressCommand(Guid TrackId,
												  int PositionSec,
												  bool Completed) : ICommand<CreateMyTrackProgressResult>;
