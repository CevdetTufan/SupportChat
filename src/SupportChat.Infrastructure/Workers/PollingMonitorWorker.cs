using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SupportChat.Application.Interfaces.Persistence;
using SupportChat.Application.Interfaces.Repositories;

namespace SupportChat.Infrastructure.Workers;

public class PollingMonitorWorker : BackgroundService
{
	private readonly IServiceProvider _services;
	private readonly ILogger<PollingMonitorWorker> _logger;
	private readonly TimeSpan _interval = TimeSpan.FromSeconds(1);

	public PollingMonitorWorker(IServiceProvider services, ILogger<PollingMonitorWorker> logger)
	{
		_services = services;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			_logger.LogInformation("Polling Monitor Worker running at: {Time}", DateTime.Now);

			using var scope = _services.CreateScope();
			var sessionRepo = scope.ServiceProvider.GetRequiredService<IChatSessionRepository>();
			var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

			await CheckForTimeoutsAsync(sessionRepo, uow, stoppingToken);

			await Task.Delay(_interval, stoppingToken);
		}
	}

	private static async Task CheckForTimeoutsAsync(
		IChatSessionRepository sessionRepo,
		IUnitOfWork uow,
		CancellationToken token)
	{
		var sessions = await sessionRepo.ListAllAsync(token);
		foreach (var session in sessions)
		{
			if (DateTime.UtcNow - session.LastPolledAt > TimeSpan.FromSeconds(3))
			{
				session.MarkInactive();
				sessionRepo.Update(session);
			}
		}
		await uow.SaveChangesAsync();
	}
}