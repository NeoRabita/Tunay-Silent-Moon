using SilentMoonApp.Application.Abstractions.Messaging;

namespace SilentMoonApp.Application.Features.CourseFavorites.Commands.DeleteMyFavorite;

public sealed record DeleteMyFavoriteCommand(Guid CourseId) : ICommand;
