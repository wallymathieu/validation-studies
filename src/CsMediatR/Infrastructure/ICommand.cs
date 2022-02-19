using MediatR;

namespace CsMediatR.Infrastructure;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}
public interface IEntityCommand<out TResponse, TEntity> : ICommand<TResponse>
{
}