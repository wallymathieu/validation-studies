using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CsMediatR.Infrastructure.CommandHandlers;

public static partial class ApiRegistrationsExtensions
{
    class FuncMutateCommandHandler<T, TCommand, TRet> : IRequestHandler<TCommand, TRet>
        where TCommand : ICommand<TRet> where T : IEntity
    {
        private readonly Func<T, TCommand, IServiceProvider, TRet> _func;
        private readonly IServiceProvider _serviceProvider;

        public FuncMutateCommandHandler(Func<T, TCommand, IServiceProvider, TRet> func, IServiceProvider serviceProvider)
        {
            _func = func;
            _serviceProvider = serviceProvider;
        }

        public async Task<TRet> Handle(TCommand cmd, CancellationToken cancellationToken)
        {
            var repository = _serviceProvider.GetRequiredService<IRepository<T>>();
            var keyValueFactory = _serviceProvider.GetRequiredService<IKeyValueFactory<TCommand>>();
            var entity = await repository.FindAsync(keyValueFactory.Key(cmd));

            var r = _func(entity, cmd, _serviceProvider);

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
            var entity = await repository.FindAsync(keyValueFactory.Key(cmd));

            _func(entity, cmd, _serviceProvider);

            return Unit.Value; 
        }
    }
}