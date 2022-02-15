using System.Linq.Expressions;
using System.Reflection;
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
            var regType = typeof(ICommandHandler<,>).MakeGenericType(commandType,returnType);
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
            var handlerTCommand = typeof(ICommandHandler<,>).MakeGenericType(commandType,returnType);
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
            // services.AddScoped<ICommandHandler<TCommand>>(svc=>new FuncMutateCommandHandler<T,TCommand>((entity, cmd, svc) => entity.`MethodInfo`(cmd, svc), svc))
            services.AddScoped(handlerTCommand, svc => lambda(svc));
        }
    }

    public class TypedAggregateRegistrationBuilder<T> where T : IEntity
    {
        private IServiceCollection services;

        public TypedAggregateRegistrationBuilder(IServiceCollection services)
        {
            this.services = services;
        }
        public TypedAggregateRegistrationBuilder<T> UpdateCommandOnEntity<TCommand,TRet>(Func<T, TCommand, IServiceProvider, TRet> func)
            where TCommand : ICommand<TRet>, IUpdateCommand
        {
            services.AddScoped<ICommandHandler<TCommand,TRet>>(di => new FuncMutateCommandHandler<T, TCommand, TRet>(func, di));
            return this;
        }
        public TypedAggregateRegistrationBuilder<T> CreateCommandOnEntity<TCommand>(Func<TCommand, IServiceProvider, T> func) 
            where TCommand : ICommand<T>
        {
            services.AddScoped<ICommandHandler<TCommand,T>>(di => new FuncCreateCommandHandler<T, TCommand>(func, di));
            return this;
        }
    }

    class FuncMutateCommandHandler<T, TCommand,TRet> : ICommandHandler<TCommand,TRet>
        where TCommand : ICommand<TRet>, IUpdateCommand where T : IEntity

    {
        private Func<T, TCommand, IServiceProvider, TRet> func;
        private IServiceProvider serviceProvider;

        public FuncMutateCommandHandler(Func<T, TCommand, IServiceProvider, TRet> func, IServiceProvider serviceProvider)
        {
            this.func = func;
            this.serviceProvider = serviceProvider;
        }

        public async Task<TRet> Handle(TCommand cmd, CancellationToken cancellationToken=default)
        {
            var repository = serviceProvider.GetRequiredService<IRepository<T>>();
            var entity = await repository.FindAsync(cmd.Identifier);

            var r = func(entity, cmd, serviceProvider);

            return r;
        }
    }

    class FuncCreateCommandHandler<T, TCommand> : ICommandHandler<TCommand,T>
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
            var entity = _func(cmd, _serviceProvider);
            await repository.AddAsync(entity);
            return entity;
        }
    }
}