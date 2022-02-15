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
        public Task AddAsync(T entity)
        {
            entities.Add(entity.Identifier,entity);
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
        services.AddMediatR(applicationAssembly);
        services.AddTransient<IRepository<Person>, InMemoryRepository<Person>>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddValidatorsFromAssembly(applicationAssembly);
    }
}