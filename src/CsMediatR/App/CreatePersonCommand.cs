using CsMediatR.Infrastructure;

namespace CsMediatR.App;

public record CreatePersonCommand(string Description) : ICommand<Person>;
