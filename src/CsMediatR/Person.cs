using FluentValidation;

namespace CsMediatR;

public class Person
{
    public string Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
    public List<Booking> Bookings { get; set; } = new List<Booking>();
}
public class Booking
{
    public string Description { get; set; }

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
        RuleForEach(n => n.Bookings).SetValidator(new BookingValidator());
    }
}
public class BookingValidator : AbstractValidator<Booking>
{
    public BookingValidator()
    {
        RuleFor(n => n.Description)
            .MinimumLength(1).WithErrorCode("DescriptionBetween1And50")
            .MaximumLength(50).WithErrorCode("DescriptionBetween1And50");
    }
}