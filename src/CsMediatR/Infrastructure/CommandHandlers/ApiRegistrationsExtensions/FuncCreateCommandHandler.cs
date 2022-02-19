using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CsMediatR.Infrastructure.CommandHandlers;

public static partial class ApiRegistrationsExtensions
{
    class FuncCreateCommandHandler<T, TCommand, TRet> : IRequestHandler<TCommand, TRet>
        where TCommand : ICommand<TRet> where T : IEntity
    {
        private readonly Func<TCommand, IServiceProvider, (T, TRet)> _func;
        private readonly IServiceProvider _serviceProvider;

        public FuncCreateCommandHandler(Func<TCommand, IServiceProvider, (T, TRet)> func,
            IServiceProvider serviceProvider)
        {
            _func = func;
            _serviceProvider = serviceProvider;
        }

        public async Task<TRet> Handle(TCommand cmd, CancellationToken cancellationToken)
        {
            var repository = _serviceProvider.GetRequiredService<IRepository<T>>();
            var validator= _serviceProvider.GetRequiredService<IValidator<T>>();
            var (entity, ret) = _func(cmd, _serviceProvider);
            await validator.ValidateAndThrowAsync(entity);
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
            var validator= _serviceProvider.GetRequiredService<IValidator<T>>();
            var entity = _func(cmd, _serviceProvider);
            await validator.ValidateAndThrowAsync(entity);
            await repository.AddAsync(entity);
            return entity;
        }
    }
}