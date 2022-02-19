using CsMediatR.Infrastructure;
using CsMediatR.Infrastructure.CommandHandlers;
using FluentValidation;

namespace CsMediatR.App;

public class Booking : IEntity
{
    public int BookingId { get; set; }
    public string Description { get; set; }
    [CommandHandler]
    public static Booking Create(CreateBookingCommand cmd, IAService services) =>
        new Booking{ Description=cmd.Description };


    [CommandHandler]
    public void Handle(EditBookingCommand cmd, IAService services)
    {
        //....
    }

}
public class BookingValidator : AbstractValidator<Booking>
{
    public BookingValidator()
    {
        RuleFor(n => n.Description)
            .NotNull().WithErrorCode("DescriptionMustNotBeNull")
            .MinimumLength(1).WithErrorCode("DescriptionBetween1And50")
            .MaximumLength(50).WithErrorCode("DescriptionBetween1And50");
    }
}