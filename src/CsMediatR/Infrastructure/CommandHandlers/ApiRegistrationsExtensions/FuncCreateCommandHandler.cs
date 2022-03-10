using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CsMediatR.Infrastructure.CommandHandlers;

public static partial class ApiRegistrationsExtensions
{
    class FuncCreateCommandHandler<TEntity, TCommand, TReturnValue> : IRequestHandler<TCommand, TReturnValue>
        where TCommand : ICommand<TReturnValue> where TEntity : IEntity
    {
        private readonly Func<TCommand, IServiceProvider, (TEntity, TReturnValue)> _func;
        private readonly IServiceProvider _serviceProvider;

        public FuncCreateCommandHandler(Func<TCommand, IServiceProvider, (TEntity, TReturnValue)> func,
            IServiceProvider serviceProvider)
        {
            _func = func;
            _serviceProvider = serviceProvider;
        }

        public async Task<TReturnValue> Handle(TCommand cmd, CancellationToken cancellationToken)
        {
            var repository = _serviceProvider.GetRequiredService<IRepository<TEntity>>();
            var (entity, ret) = _func(cmd, _serviceProvider);
            await repository.AddAsync(entity);
            return ret;
        }
    }

    class FuncCreateCommandHandler<T, TCommand> : IRequestHandler<TCommand, T>
        where TCommand : ICommand<T> where T : IEntity
    {
        private readonly Func<TCommand, IServiceProvider, T> _func;
        private readonly IServiceProvider _serviceProvider;

        public FuncCreateCommandHandler(Func<TCommand, IServiceProvider, T> func, IServiceProvider serviceProvider)
        {
            _func = func;
            _serviceProvider = serviceProvider;
        }

        public async Task<T> Handle(TCommand cmd, CancellationToken cancellationToken)
        {
            var repository = _serviceProvider.GetRequiredService<IRepository<T>>();
            var entity = _func(cmd, _serviceProvider);
            await repository.AddAsync(entity);
            return entity;
        }
    }
}