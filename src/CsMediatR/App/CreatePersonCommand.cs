using CsMediatR.Infrastructure;

namespace CsMediatR.App;

public record CreatePersonCommand(string Description) : IEntityCommand<Person,Person>;
