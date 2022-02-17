namespace CsMediatR.Infrastructure;

public interface IRepository<T>
    where T:IEntity
{
    Task AddAsync(T entity);
    Task<T> FindAsync(object identifier);
}