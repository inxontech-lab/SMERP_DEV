namespace SMERP.services.SaasServices;

public interface ICrudApiClient<TEntity> where TEntity : class
{
    Task<IReadOnlyList<TEntity>?> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TEntity?> CreateAsync(TEntity model, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, TEntity model, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
