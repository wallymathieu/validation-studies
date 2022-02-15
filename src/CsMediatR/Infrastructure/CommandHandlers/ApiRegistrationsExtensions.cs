using System.Linq.Expressions;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CsMediatR.Infrastructure.CommandHandlers;

public static class ApiRegistrationsExtensions
{
    public static TypedAggregateRegistrationBuilder<T> RegisterHandlersFor<T>(this IServiceCollection services) where T : IEntity
    {
        return new TypedAggregateRegistrationBuilder<T>(services);
    }

    public static IServiceCollection RegisterAttributesForType<T>(this IServiceCollection services) where T :IEntity
    {
        return RegisterAttributesForType(services, typeof(T));
    }
    public static IServiceCollection RegisterAttributesForType(this IServiceCollection services, Type t)
    {
        RegisterCreateCommandHandlers(services, t);
        RegisterUpdateCommandHandlers(services, t);
        return services;
    }

    private static void RegisterCreateCommandHandlers(IServiceCollection services, Type t)
    {
        foreach (var (methodInfo, commandType, returnType) in
                 from method in t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                 let parameters = method.GetParameters()
                 let attr = method.GetCustomAttribute<CreateCommandHandlerAttribute>()
                 where attr != null
                       && method.ReturnType == t
                       && parameters.Length == 2
                       && parameters[1].ParameterType == typeof(IServiceProvider)
                 // has the correct signature
                 select (method, parameters[0].ParameterType, method.ReturnType))
        {
            var regType = typeof(IRequestHandler<,>).MakeGenericType(commandType,returnType);
            var implType = typeof(FuncCreateCommandHandler<,>).MakeGenericType(t, commandType);
            //<TCommand, IServiceProvider, T>
            var funcType = typeof(Func<,,>).MakeGenericType(commandType, typeof(IServiceProvider), t);
            var parameter = Expression.Parameter(typeof(IServiceProvider), "di");
            // (cmd, svc) => `EntityType`.`MethodInfo`(cmd, svc)
            Func<IServiceProvider, object> lambda = Expression.Lambda<Func<IServiceProvider, object>>(
                Expression.New( // new
                    implType.GetConstructors()[0], // FuncCreateCommandHandler<T,TCommand>
                    Expression.Constant(Delegate.CreateDelegate(funcType, methodInfo)), //
                    parameter
                ),
                parameter).Compile();
            services.AddScoped(regType, svc => lambda(svc));
        }
    }

    private static void RegisterUpdateCommandHandlers(IServiceCollection services, Type t)
    {
        foreach (var (methodInfo, commandType, returnType) in
                 from method in t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                 let parameters = method.GetParameters()
                 let attr = method.GetCustomAttribute<MutateCommandHandlerAttribute>()
                 where attr != null
                       && parameters.Length == 2
                       && parameters[1].ParameterType == typeof(IServiceProvider)
                 // has the correct signature
                 select (method, parameters[0].ParameterType, method.ReturnType))
        {
            var handlerTCommand = typeof(IRequestHandler<,>).MakeGenericType(commandType,returnType);
            var funcMutateCommandHandlerTT = typeof(FuncMutateCommandHandler<,,>).MakeGenericType(t, commandType, returnType);
            var parameter_Entity = Expression.Parameter(t, "entity");
            var parameter_Cmd = Expression.Parameter(commandType, "cmd");
            var parameter_Svc = Expression.Parameter(typeof(IServiceProvider), "svc");
            // (entity, cmd, svc) => entity.`MethodInfo`(cmd, svc)
            Func<IServiceProvider, object> lambda = Expression.Lambda<Func<IServiceProvider, object>>(
                    Expression.New( // new  
                        funcMutateCommandHandlerTT.GetConstructors()[0], // FuncMutateCommandHandler<T,TCommand,TRet>
                        Expression.Lambda( // (entity, cmd, svc) =>
                            Expression.Call(parameter_Entity, methodInfo, parameter_Cmd, parameter_Svc), // entity.`MethodInfo`(cmd, svc) , i.e. entity.HandleTheCommand(cmd,svc)
                            parameter_Entity, parameter_Cmd, parameter_Svc),
                        parameter_Svc
                    ),
                    parameter_Svc)
                .Compile();
            // services.AddScoped<IRequestHandler<TCommand>>(svc=>new FuncMutateCommandHandler<T,TCommand>((entity, cmd, svc) => entity.`MethodInfo`(cmd, svc), svc))
            services.AddScoped(handlerTCommand, svc => lambda(svc));
        }
    }

    public class TypedAggregateRegistrationBuilder<T> where T : IEntity
    {
        private readonly IServiceCollection _services;

        public TypedAggregateRegistrationBuilder(IServiceCollection services) => _services = services;

        public TypedAggregateRegistrationBuilder<T> UpdateCommandOnEntity<TCommand,TRet>(Func<T, TCommand, IServiceProvider, TRet> func)
            where TCommand : ICommand<TRet>, IUpdateCommand
        {
            _services.AddScoped<IRequestHandler<TCommand,TRet>>(di => new FuncMutateCommandHandler<T, TCommand, TRet>(func, di));
            return this;
        }
        public TypedAggregateRegistrationBuilder<T> CreateCommandOnEntity<TCommand>(Func<TCommand, IServiceProvider, T> func) 
            where TCommand : ICommand<T>
        {
            _services.AddScoped<IRequestHandler<TCommand,T>>(di => new FuncCreateCommandHandler<T, TCommand>(func, di));
            return this;
        }
        public TypedAggregateRegistrationBuilder<T> CreateCommandOnEntity<TCommand,TRet>(Func<TCommand, IServiceProvider, (T,TRet)> func) 
            where TCommand : ICommand<TRet>
        {
            _services.AddScoped<IRequestHandler<TCommand,TRet>>(di => new FuncCreateCommandHandler<T, TCommand,TRet>(func, di));
            return this;
        }
    }

    class FuncMutateCommandHandler<T, TCommand,TRet> : IRequestHandler<TCommand,TRet>
        where TCommand : ICommand<TRet>, IUpdateCommand where T : IEntity
    {
        private readonly Func<T, TCommand, IServiceProvider, TRet> _func;
        private readonly IServiceProvider _serviceProvider;

        public FuncMutateCommandHandler(Func<T, TCommand, IServiceProvider, TRet> func, IServiceProvider serviceProvider)
        {
            _func = func;
            _serviceProvider = serviceProvider;
        }

        public async Task<TRet> Handle(TCommand cmd, CancellationToken cancellationToken=default)
        {
            var repository = _serviceProvider.GetRequiredService<IRepository<T>>();
            var entity = await repository.FindAsync(cmd.Identifier);

            var r = _func(entity, cmd, _serviceProvider);

            return r;
        }
    }

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

        public async Task<TRet> Handle(TCommand cmd, CancellationToken cancellationToken = default)
        {
            var repository = _serviceProvider.GetRequiredService<IRepository<T>>();
            var (entity, ret) = _func(cmd, _serviceProvider);
            await repository.AddAsync(entity);
            return ret;
        }
    }

    class FuncCreateCommandHandler<T, TCommand> : IRequestHandler<TCommand,T>
        where TCommand : ICommand<T> where T : IEntity
    {
        private readonly Func<TCommand, IServiceProvider, T> _func;
        private readonly IServiceProvider _serviceProvider;

        public FuncCreateCommandHandler(Func<TCommand, IServiceProvider, T> func, IServiceProvider serviceProvider)
        {
            _func = func;
            _serviceProvider = serviceProvider;
        }

        public async Task<T> Handle(TCommand cmd, CancellationToken cancellationToken=default)
        {
            var repository = _serviceProvider.GetRequiredService<IRepository<T>>();
            var entity= _func(cmd, _serviceProvider);
            await repository.AddAsync(entity);
            return entity;
        }
    }
}