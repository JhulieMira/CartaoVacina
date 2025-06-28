using CartaoVacina.Contracts.Data.Entities;

namespace CartaoVacina.Contracts.Data.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetById(int id);
    IEnumerable<T> Get(Func<T, bool> predicate);
    IEnumerable<T> Get();
    Task Add(T entity);
    void Update(T entity);
    Task Delete(int id);
    Task<int> Count();
}