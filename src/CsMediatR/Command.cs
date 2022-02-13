using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CsFluent;

public class Command
{
    
}

public class Startup
{

    public void ConfigureServices(IServiceCollection services)
    {
        var applicationAssembly = this.GetType().Assembly;
        services.AddMediatR(applicationAssembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddValidatorsFromAssembly(applicationAssembly);
    }
}

public class ValidationBehavior<T, T1>
{
}