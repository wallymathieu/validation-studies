using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CsMediatR.Infrastructure.CommandHandlers;

public static partial class ApiRegistrationsExtensions
{
    class FuncMutateCommandHandler<TEntity, TCommand, TReturnvalue> : IRequestHandler<TCommand, TReturnvalue>
        where TCommand : ICommand<TReturnvalue> where TEntity : IEntity
    {
        private readonly Func<TEntity, TCommand, IServiceProvider, TReturnvalue> _func;
        private readonly IServiceProvider _serviceProvider;

        public FuncMutateCommandHandler(Func<TEntity, TCommand, IServiceProvider, TReturnvalue> func, IServiceProvider serviceProvider)
        {
            _func = func;
            _serviceProvider = serviceProvider;
        }

        public async Task<TReturnvalue> Handle(TCommand cmd, CancellationToken cancellationToken)
        {
            var repository = _serviceProvider.GetRequiredService<IRepository<TEntity>>();
            var keyValueFactory = _serviceProvider.GetRequiredService<IKeyValueFactory<TCommand>>();
            var entity = await repository.FindAsync(keyValueFactory.Key(cmd));

            var r = _func(entity, cmd, _serviceProvider);

            return r;
        }
    }

}