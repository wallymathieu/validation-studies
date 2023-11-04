using System.Reflection;
using CsMediatR.Infrastructure;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CsMediatR.App;

public class Startup
{
    class InMemoryRepository<T> : IRepository<T>
        where T:IEntity
    {
        private IDictionary<object, T> entities = new Dictionary<object, T>();
        private IKeyValueFactory<T> keyValueFactory;
        public InMemoryRepository(IKeyValueFactory<T> keyValueFactory)=>this.keyValueFactory=keyValueFactory;
        public Task AddAsync(T entity)
        {
            entities.Add(keyValueFactory.Key(entity),entity);
            return Task.CompletedTask;
        }

        public Task<T> FindAsync(object identifier)
        {
            return Task.FromResult(entities[identifier]);
        }
    } 
    public void ConfigureServices(IServiceCollection services)
    {
        var applicationAssembly = this.GetType().Assembly;
        services.AddMediatR(cfg=>cfg.RegisterServicesFromAssemblies(applicationAssembly));
        services.AddTransient<IAService,AService>();
        services.AddTransient<IRepository<Person>, InMemoryRepository<Person>>();
        services.AddTransient<IRepository<Booking>, InMemoryRepository<Booking>>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddSingleton(typeof(IKeyValueFactory<>), typeof(KeyValueFactory<>));
        services.AddValidatorsFromAssembly(applicationAssembly);
    }

    private class AService:IAService
    {
    }
}
