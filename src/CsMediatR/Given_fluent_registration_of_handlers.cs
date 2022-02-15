using CsMediatR.App;
using CsMediatR.Infrastructure.CommandHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CsMediatR;

public class Given_fluent_registration_of_handlers
{
    private readonly ServiceProvider _serviceProvider;
    public Given_fluent_registration_of_handlers()
    {
        var conf = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(conf);
        services.AddOptions();

        services.RegisterHandlersFor<Person>()
            .UpdateCommandOnEntity<EditPersonCommand,Person>((entity, cmd, svc) => entity.Handle(cmd, svc))
            .CreateCommandOnEntity<CreatePersonCommand>(Person.Create)
            ;

        var startup = new Startup();
        startup.ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }
    [Fact]
    public async Task Can_find_and_execute_create()
    {
        var handler = _serviceProvider.GetRequiredService<ICommandHandler<CreatePersonCommand,Person>>();
        await handler.Handle(new CreatePersonCommand("Description"),default);
    }
    [Fact]
    public void Can_find_update()
    {
        _serviceProvider.GetRequiredService<ICommandHandler<EditPersonCommand,Person>>();
    }
    [Fact]
    public async Task Validation_is_triggered()
    {
        var handler= _serviceProvider.GetRequiredService<ICommandHandler<EditPersonCommand,Person>>();
        
        await handler.Handle(new EditPersonCommand(null, 1),default);
    }
}