using Microsoft.EntityFrameworkCore;
using SupportChat.Application.Interfaces.Repositories;
using SupportChat.Infrastructure.Data;

namespace SupportChat.Infrastructure.Repositories;

internal class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
	protected readonly AppDbContext _context;
	private readonly DbSet<TEntity> _dbSet;

	public Repository(AppDbContext context)
	{
		_context = context ?? throw new ArgumentNullException(nameof(context));
		_dbSet = _context.Set<TEntity>();
	}

	public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken token = default)
	{
		return await _dbSet.FindAsync(id, token);
	}

	public async Task<IReadOnlyList<TEntity>> ListAllAsync(CancellationToken token = default)
	{
		return await _dbSet.ToListAsync(token);
	}

	public async Task AddAsync(TEntity entity, CancellationToken token = default)
	{
		await _dbSet.AddAsync(entity, token);
	}

	public void Update(TEntity entity)
	{
		_dbSet.Update(entity);
	}

	public void Delete(TEntity entity)
	{
		_dbSet.Remove(entity);
	}
}
