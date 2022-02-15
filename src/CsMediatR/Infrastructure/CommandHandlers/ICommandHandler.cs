using MediatR;

namespace CsMediatR.Infrastructure.CommandHandlers;

public interface ICommandHandler<in TCommand,TResponse> : IRequestHandler<TCommand,TResponse>
    where TCommand:ICommand<TResponse>
{
    
}