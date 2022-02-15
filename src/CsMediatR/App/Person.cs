using CsMediatR.Infrastructure;
using CsMediatR.Infrastructure.CommandHandlers;
using FluentValidation;

namespace CsMediatR.App;

public class Person:IEntity
{
    public string Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
        
    [CreateCommandHandler]
    public static Person Create(CreatePersonCommand cmd, IServiceProvider services) =>
        new Person();


    [MutateCommandHandler]
    public Person Handle(EditPersonCommand cmd, IServiceProvider services) =>
        //....
        this;

    object IEntity.Identifier => Id;
    public int Id { get; set; }
}
public class PersonValidator : AbstractValidator<Person>
{
    public PersonValidator()
    {
        RuleFor(n => n.Name)
            .MinimumLength(1).WithErrorCode("NameBetween1And50")
            .MaximumLength(50).WithErrorCode("NameBetween1And50");
        RuleFor(n => n.Email).EmailAddress().WithErrorCode("EmailMustContainAtChar");
        RuleFor(n => n.Age).InclusiveBetween(0, 120).WithErrorCode("AgeBetween0and120");
    }
}