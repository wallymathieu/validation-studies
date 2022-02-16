using CsMediatR.Infrastructure;
using FluentValidation;

namespace CsMediatR.App;

public record EditPersonCommand(string Description, int Id):ICommand<Person>
{
}
public class EditPersonCommandValidator: AbstractValidator<EditPersonCommand>
{
    public EditPersonCommandValidator()
    {
        RuleFor(x => x.Description).NotNull().NotEmpty().WithMessage("Must have a description");
    }
}
