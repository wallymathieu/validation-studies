using CsMediatR.Infrastructure;

namespace CsMediatR.App;

public record CreateBookingCommand(string Description) : IEntityCommand<Booking,Booking>;