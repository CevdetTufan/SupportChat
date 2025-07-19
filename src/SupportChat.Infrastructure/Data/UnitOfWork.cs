using SupportChat.Application.Interfaces.Persistence;

namespace SupportChat.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
	private readonly AppDbContext _dbContext;

	public UnitOfWork(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public Task SaveChangesAsync()
	{
		return _dbContext.SaveChangesAsync();
	}
}
