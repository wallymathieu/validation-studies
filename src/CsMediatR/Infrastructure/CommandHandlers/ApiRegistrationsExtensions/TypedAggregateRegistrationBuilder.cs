using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CsMediatR.Infrastructure.CommandHandlers;

public static partial class ApiRegistrationsExtensions
{
    public class TypedAggregateRegistrationBuilder<TEntity> where TEntity : IEntity
    {
        private readonly IServiceCollection _services;

        public TypedAggregateRegistrationBuilder(IServiceCollection services) => _services = services;

        public TypedAggregateRegistrationBuilder<TEntity> UpdateCommandOnEntity<TCommand, TReturnvalue>(Func<TEntity, TCommand, IServiceProvider, TReturnvalue> func)
            where TCommand : ICommand<TReturnvalue>
        {
            _services.AddScoped<IRequestHandler<TCommand, TReturnvalue>>(di => new FuncMutateCommandHandler<TEntity, TCommand, TReturnvalue>(func, di));
            return this;
        }
        public TypedAggregateRegistrationBuilder<TEntity> CreateCommandOnEntity<TCommand>(Func<TCommand, IServiceProvider, TEntity> func)
            where TCommand : ICommand<TEntity>
        {
            _services.AddScoped<IRequestHandler<TCommand, TEntity>>(di => new FuncCreateCommandHandler<TEntity, TCommand>(func, di));
            return this;
        }
        public TypedAggregateRegistrationBuilder<TEntity> CreateCommandOnEntity<TCommand, TReturnValue>(Func<TCommand, IServiceProvider, (TEntity, TReturnValue)> func)
            where TCommand : ICommand<TReturnValue>
        {
            _services.AddScoped<IRequestHandler<TCommand, TReturnValue>>(di => new FuncCreateCommandHandler<TEntity, TCommand, TReturnValue>(func, di));
            return this;
        }
    }
}