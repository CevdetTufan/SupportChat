namespace SupportChat.Application.Interfaces.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
	Task<TEntity?> GetByIdAsync(Guid id, CancellationToken token = default);
	Task<IReadOnlyList<TEntity>> ListAllAsync(CancellationToken token = default);
	Task AddAsync(TEntity entity, CancellationToken token = default);
	void Update(TEntity entity);
	void Delete(TEntity entity);
}
