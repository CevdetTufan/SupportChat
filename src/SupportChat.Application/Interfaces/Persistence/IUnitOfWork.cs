namespace SupportChat.Application.Interfaces.Persistence;

public interface IUnitOfWork
{
	Task SaveChangesAsync();
}
