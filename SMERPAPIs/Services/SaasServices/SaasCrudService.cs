using Domain.SaasDBModels;
using Microsoft.EntityFrameworkCore;

namespace SMERPAPIs.Services.SaasServices;

public class SaasCrudService<TEntity, TRequest, TKey> : ISaasCrudService<TEntity, TRequest, TKey>
    where TEntity : class, new()
{
    private readonly SmerpContext _context;
    private readonly DbSet<TEntity> _dbSet;
    private readonly Action<TEntity, TRequest> _mapRequest;

    public SaasCrudService(SmerpContext context, Action<TEntity, TRequest> mapRequest)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
        _mapRequest = mapRequest;
    }

    public async Task<List<TEntity>> GetAllAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public async Task<TEntity?> GetByIdAsync(TKey id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<TEntity> CreateAsync(TRequest request)
    {
        var entity = new TEntity();
        _mapRequest(entity, request);
        _dbSet.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> UpdateAsync(TKey id, TRequest request)
    {
        var existingEntity = await _dbSet.FindAsync(id);
        if (existingEntity is null)
        {
            return false;
        }

        _mapRequest(existingEntity, request);
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
