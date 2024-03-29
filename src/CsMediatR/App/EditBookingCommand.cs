using CsMediatR.Infrastructure;
using FluentValidation;

namespace CsMediatR.App;

public record EditBookingCommand(string Description, int Id):ICommand<MediatR.Unit>
{
}
public class EditBookingCommandValidator: AbstractValidator<EditBookingCommand>
{
    public EditBookingCommandValidator()
    {
        RuleFor(x => x.Description).NotNull().NotEmpty().WithMessage("Must have a description");
    }
}
