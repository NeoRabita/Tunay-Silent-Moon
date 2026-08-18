using SilentMoonApp.Application.Messaging;


namespace SilentMoonApp.Application.Abstractions.Messaging;


public interface ICommandBase { }

public interface ICommand : IRequest<NoResult>, ICommandBase { }

public interface ICommand<TResponse> : IRequest<TResponse>, ICommandBase { }

public interface INonTransactionalCommand { }

public interface INonLoggableCommand { }