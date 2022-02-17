namespace CsMediatR.Infrastructure;

public interface IKeyValueFactory<T>
{
  object? Key(T obj);
}
