using CsMediatR.Infrastructure;

namespace CsMediatR.App;

public record CreateBookingCommand(string Description) : ICommand<Booking>;