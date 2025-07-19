using Microsoft.Extensions.Hosting;
using SupportChat.Application.Interfaces.Persistence;
using SupportChat.Application.Interfaces.Repositories;

namespace SupportChat.Infrastructure.Workers;

public class PollingMonitorWorker : BackgroundService
{
	private readonly IChatSessionRepository _sessionRepo;
	private readonly IUnitOfWork _uow;

	private readonly TimeSpan _interval = TimeSpan.FromSeconds(1);

	public PollingMonitorWorker(IChatSessionRepository sessionRepo, IUnitOfWork uow)
	{
		_sessionRepo = sessionRepo;
		_uow = uow;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			await CheckForTimeoutsAsync(stoppingToken);
			await Task.Delay(_interval, stoppingToken);
		}
	}

	private async Task CheckForTimeoutsAsync(CancellationToken token)
	{
		var sessions = await _sessionRepo.ListAllAsync(token);
		foreach (var session in sessions)
		{
			if (DateTime.UtcNow - session.LastPolledAt > TimeSpan.FromSeconds(3))
			{
				session.MarkInactive();
				_sessionRepo.Update(session);
			}
		}
		await _uow.SaveChangesAsync();
	}
}