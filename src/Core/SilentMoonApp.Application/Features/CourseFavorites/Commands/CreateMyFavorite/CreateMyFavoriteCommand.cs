using SilentMoonApp.Application.Abstractions.Messaging;
using System.Windows.Input;

namespace SilentMoonApp.Application.Features.CourseFavorites.Commands.CreateMyFavorite;

public sealed record CreateMyFavoriteCommand(Guid CourseId) : ICommand<CreateMyFavoriteResult>;

