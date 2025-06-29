using CartaoVacina.Contracts.Data.Entities;
using CartaoVacina.Contracts.Data.Repositories;
using CartaoVacina.Migrations;
using Microsoft.EntityFrameworkCore;

namespace CartaoVacina.Infrastructure.Data.Repositories;

public class Repository<T>(DatabaseContext context) : IRepository<T> where T : BaseEntity
{
    private readonly DbSet<T> _dbSet = context.Set<T>();

    public async Task<T?> GetById(int id, CancellationToken cancellationToken = default)
    {
        var findParams = new object[] { id };
        return await _dbSet.FindAsync(findParams, cancellationToken);
    }

    public IEnumerable<T> Get(Func<T, bool> predicate)
    {
        return _dbSet.AsQueryable().Where(predicate);
    }

    public IQueryable<T> Get()
    {
        return _dbSet.AsQueryable();
    }

    public async Task Add(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public async Task Delete(int id, CancellationToken cancellationToken = default)
    {
        if (id == 0)
            return;
        
        var entity = await GetById(id, cancellationToken);
        
        if (entity == null)
            return;

        _dbSet.Remove(entity);
    }

    public async Task<int> Count(CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(cancellationToken);
    }
}