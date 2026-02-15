using System.Collections.Generic;

namespace SMERPAPIs.Services.SaasServices;

public interface ISaasCrudService<TEntity, TKey> where TEntity : class
{
    Task<List<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(TKey id);
    Task<TEntity> CreateAsync(TEntity entity);
    Task<bool> UpdateAsync(TKey id, TEntity entity);
    Task<bool> DeleteAsync(TKey id);
}
