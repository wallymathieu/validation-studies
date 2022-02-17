using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CsMediatR.Infrastructure.CommandHandlers;

public static partial class ApiRegistrationsExtensions
{
    public class TypedAggregateRegistrationBuilder<T> where T : IEntity
    {
        private readonly IServiceCollection _services;

        public TypedAggregateRegistrationBuilder(IServiceCollection services) => _services = services;

        public TypedAggregateRegistrationBuilder<T> UpdateCommandOnEntity<TCommand, TRet>(Func<T, TCommand, IServiceProvider, TRet> func)
            where TCommand : ICommand<TRet>
        {
            _services.AddScoped<IRequestHandler<TCommand, TRet>>(di => new FuncMutateCommandHandler<T, TCommand, TRet>(func, di));
            return this;
        }
        public TypedAggregateRegistrationBuilder<T> CreateCommandOnEntity<TCommand>(Func<TCommand, IServiceProvider, T> func)
            where TCommand : ICommand<T>
        {
            _services.AddScoped<IRequestHandler<TCommand, T>>(di => new FuncCreateCommandHandler<T, TCommand>(func, di));
            return this;
        }
        public TypedAggregateRegistrationBuilder<T> CreateCommandOnEntity<TCommand, TRet>(Func<TCommand, IServiceProvider, (T, TRet)> func)
            where TCommand : ICommand<TRet>
        {
            _services.AddScoped<IRequestHandler<TCommand, TRet>>(di => new FuncCreateCommandHandler<T, TCommand, TRet>(func, di));
            return this;
        }
    }
}