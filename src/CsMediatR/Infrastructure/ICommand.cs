using MediatR;

namespace CsMediatR.Infrastructure;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}