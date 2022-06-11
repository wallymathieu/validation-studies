using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CsMediatR.Infrastructure.CommandHandlers;

public static partial class ApiRegistrationsExtensions
{
    class FuncCreateCommandHandler<TEntity, TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse> where TEntity : IEntity
    {
        private readonly Func<TCommand, IServiceProvider, (TEntity, TResponse)> _func;
        private readonly IServiceProvider _serviceProvider;

        public FuncCreateCommandHandler(Func<TCommand, IServiceProvider, (TEntity, TResponse)> func,
            IServiceProvider serviceProvider)
        {
            _func = func;
            _serviceProvider = serviceProvider;
        }

        public async Task<TResponse> Handle(TCommand cmd, CancellationToken cancellationToken)
        {
            var repository = _serviceProvider.GetRequiredService<IRepository<TEntity>>();
            var validator= _serviceProvider.GetRequiredService<IValidator<TEntity>>();
            var (entity, ret) = _func(cmd, _serviceProvider);
            await validator.ValidateAndThrowAsync(entity);
            await repository.AddAsync(entity);
            return ret;
        }
    }

    class FuncCreateCommandHandler<TEntity, TCommand> : IRequestHandler<TCommand, TEntity>
        where TCommand : ICommand<TEntity> where TEntity : IEntity
    {
        private readonly Func<TCommand, IServiceProvider, TEntity> _func;
        private readonly IServiceProvider _serviceProvider;

        public FuncCreateCommandHandler(Func<TCommand, IServiceProvider, TEntity> func, IServiceProvider serviceProvider)
        {
            _func = func;
            _serviceProvider = serviceProvider;
        }

        public async Task<TEntity> Handle(TCommand cmd, CancellationToken cancellationToken)
        {
            var repository = _serviceProvider.GetRequiredService<IRepository<TEntity>>();
            var validator= _serviceProvider.GetRequiredService<IValidator<TEntity>>();
            var entity = _func(cmd, _serviceProvider);
            await validator.ValidateAndThrowAsync(entity);
            await repository.AddAsync(entity);
            return entity;
        }
    }
}