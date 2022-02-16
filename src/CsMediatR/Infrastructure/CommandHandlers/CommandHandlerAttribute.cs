namespace CsMediatR.Infrastructure.CommandHandlers;

[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public sealed class CommandHandlerAttribute : Attribute
{
}