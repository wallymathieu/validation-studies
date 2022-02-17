using CsMediatR.App;
using CsMediatR.Infrastructure.CommandHandlers;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CsMediatR;

public class Given_fluent_registration_of_handlers
{
    private readonly ServiceProvider _serviceProvider;
    private readonly IMediator _mediator;

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
        _mediator = _serviceProvider.GetRequiredService<IMediator>();
    }
    
    [Fact]
    public void Can_find_create() => Assert.NotNull(_serviceProvider.GetRequiredService<IRequestHandler<CreatePersonCommand, Person>>());

    [Fact]
    public async Task Can_execute_create() => await _mediator.Send(new CreatePersonCommand("Description"),default);

    [Fact]
    public void Can_find_update_handler() => Assert.NotNull( _serviceProvider.GetRequiredService<IRequestHandler<EditPersonCommand,Person>>());

    [Fact]
    public async Task Validation_is_triggered() =>
        await Assert.ThrowsAsync<ValidationException>(async ()=>
            await _mediator.Send(new EditPersonCommand(null, 1), default));
}