using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CsMediatR.Infrastructure.CommandHandlers;

public static partial class ApiRegistrationsExtensions
{
    class FuncMutateCommandHandler<TEntity, TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse> where TEntity : IEntity
    {
        private readonly Func<TEntity, TCommand, IServiceProvider, TResponse> _func;
        private readonly IServiceProvider _serviceProvider;

        public FuncMutateCommandHandler(Func<TEntity, TCommand, IServiceProvider, TResponse> func, IServiceProvider serviceProvider)
        {
            _func = func;
            _serviceProvider = serviceProvider;
        }

        public async Task<TResponse> Handle(TCommand cmd, CancellationToken cancellationToken)
        {
            var repository = _serviceProvider.GetRequiredService<IRepository<TEntity>>();
            var keyValueFactory = _serviceProvider.GetRequiredService<IKeyValueFactory<TCommand>>();
            var validator= _serviceProvider.GetRequiredService<IValidator<T>>();
            var entity = await repository.FindAsync(keyValueFactory.Key(cmd));

            var r = _func(entity, cmd, _serviceProvider);
            await validator.ValidateAndThrowAsync(entity);

            return r;
        }
    }
    class FuncMutateCommandHandler<T, TCommand> : IRequestHandler<TCommand, MediatR.Unit>
        where TCommand : ICommand<MediatR.Unit> where T : IEntity
    {
        private readonly Action<T, TCommand, IServiceProvider> _func;
        private readonly IServiceProvider _serviceProvider;

        public FuncMutateCommandHandler(Action<T, TCommand, IServiceProvider> func, IServiceProvider serviceProvider)
        {
            _func = func;
            _serviceProvider = serviceProvider;
        }

        public async Task<MediatR.Unit> Handle(TCommand cmd, CancellationToken cancellationToken)
        {
            var repository = _serviceProvider.GetRequiredService<IRepository<T>>();
            var keyValueFactory = _serviceProvider.GetRequiredService<IKeyValueFactory<TCommand>>();
            var validator= _serviceProvider.GetRequiredService<IValidator<T>>();
            var entity = await repository.FindAsync(keyValueFactory.Key(cmd));

            _func(entity, cmd, _serviceProvider);
            await validator.ValidateAndThrowAsync(entity);
            return Unit.Value; 
        }
    }
}