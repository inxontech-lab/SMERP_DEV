using Domain.SaasDBModels;
using Microsoft.EntityFrameworkCore;

namespace SMERPAPIs.Services.SaasServices;

public class SaasCrudService<TEntity, TKey> : ISaasCrudService<TEntity, TKey> where TEntity : class
{
    private readonly SmerpContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public SaasCrudService(SmerpContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<List<TEntity>> GetAllAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public async Task<TEntity?> GetByIdAsync(TKey id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<TEntity> CreateAsync(TEntity entity)
    {
        _dbSet.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> UpdateAsync(TKey id, TEntity entity)
    {
        var existingEntity = await _dbSet.FindAsync(id);
        if (existingEntity is null)
        {
            return false;
        }

        _context.Entry(existingEntity).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(TKey id)
    {
        var existingEntity = await _dbSet.FindAsync(id);
        if (existingEntity is null)
        {
            return false;
        }

        _dbSet.Remove(existingEntity);
        await _context.SaveChangesAsync();
        return true;
    }
}
