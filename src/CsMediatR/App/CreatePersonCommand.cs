using CsMediatR.Infrastructure;

namespace CsMediatR.App;

public record CreatePersonCommand(string Name, string Email, int Age) : ICommand<Person>;
