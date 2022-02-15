namespace CsMediatR.Infrastructure;

public interface IUpdateCommand
{
    object Identifier { get; }
}