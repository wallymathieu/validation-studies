using CsMediatR.Infrastructure;
using CsMediatR.Infrastructure.CommandHandlers;
using FluentValidation;

namespace CsMediatR.App;

public class Person:IEntity
{
    public string Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
        
    [CommandHandler]
    public static Person Create(CreatePersonCommand cmd, IServiceProvider services) =>
        new Person{ Name=cmd.Name,Email=cmd.Email,Age=cmd.Age };


    [CommandHandler]
    public Person Handle(EditPersonCommand cmd, IServiceProvider services) =>
        //....
        this;

    public int Id { get; set; }
}
public class PersonValidator : AbstractValidator<Person>
{
    public PersonValidator()
    {
        RuleFor(n => n.Name)
            .NotNull().WithErrorCode("NameMustNotBeNull")
            .MinimumLength(1).WithErrorCode("NameBetween1And50")
            .MaximumLength(50).WithErrorCode("NameBetween1And50");
        RuleFor(n => n.Email)
            .NotNull().WithErrorCode("EmailMustNotBeNull")
            .EmailAddress().WithErrorCode("EmailMustContainAtChar");
        RuleFor(n => n.Age).InclusiveBetween(0, 120).WithErrorCode("AgeBetween0and120");
    }
}
