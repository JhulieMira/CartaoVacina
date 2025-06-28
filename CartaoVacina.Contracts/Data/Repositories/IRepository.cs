using CartaoVacina.Contracts.Data.Entities;

namespace CartaoVacina.Contracts.Data.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetById(int id, CancellationToken cancellationToken = default);
    IEnumerable<T> Get(Func<T, bool> predicate);
    IEnumerable<T> Get();
    Task Add(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    Task Delete(int id, CancellationToken cancellationToken = default);
    Task<int> Count(CancellationToken cancellationToken = default);
}