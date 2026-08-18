namespace SilentMoonApp.Application.Abstractions.Messaging;


public interface IQueryBase { }
public interface IQuery<out TResponse> : IRequest<TResponse>, IQueryBase { }

