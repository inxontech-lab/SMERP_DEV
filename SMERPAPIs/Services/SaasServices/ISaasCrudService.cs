using System.Collections.Generic;

namespace SMERPAPIs.Services.SaasServices;

public interface ISaasCrudService<TEntity, TRequest, TKey> where TEntity : class
{
    Task<List<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(TKey id);
    Task<TEntity> CreateAsync(TRequest request);
    Task<bool> UpdateAsync(TKey id, TRequest request);
    Task<bool> DeleteAsync(TKey id);
}
