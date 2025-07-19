using Microsoft.Extensions.Hosting;
using SupportChat.Application.Interfaces.Coordination;

namespace SupportChat.Infrastructure.Workers;

public class ChatCoordinatorWorker : BackgroundService
{
	private readonly IChatCoordinatorEngine _engine;

	public ChatCoordinatorWorker(IChatCoordinatorEngine engine)
		=> _engine = engine;

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			await _engine.ProcessNextSessionAsync();
			await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
		}
	}
}
