using CartaoVacina.Contracts.Data.Entities;
using CartaoVacina.Contracts.Data.Repositories;
using CartaoVacina.Migrations;
using Microsoft.EntityFrameworkCore;

namespace CartaoVacina.Infrastructure.Data.Repositories;

public class Repository<T>(DatabaseContext context) : IRepository<T> where T : BaseEntity
{
    private readonly DbSet<T> _dbSet = context.Set<T>();

    public async Task<T?> GetById(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public IEnumerable<T> Get(Func<T, bool> predicate)
    {
        return _dbSet.Where(predicate);
    }

    public IEnumerable<T> Get()
    {
        return _dbSet.AsEnumerable();
    }

    public async Task Add(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public async Task Delete(int id)
    {
        if (id == 0)
            return;
        
        var entity = await GetById(id);
        
        if (entity == null)
            return;

        _dbSet.Remove(entity);
    }

    public async Task<int> Count()
    {
        return await _dbSet.CountAsync();
    }
}