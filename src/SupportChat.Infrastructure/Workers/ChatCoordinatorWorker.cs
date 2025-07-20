using Autofac.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SupportChat.Application.Interfaces.Coordination;

namespace SupportChat.Infrastructure.Workers;

public class ChatCoordinatorWorker : BackgroundService
{
	private readonly IServiceProvider _services;
	private readonly ILogger<ChatCoordinatorWorker> _logger;

	public ChatCoordinatorWorker(IServiceProvider servicec, ILogger<ChatCoordinatorWorker> logger)
	{
		_services = servicec;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			_logger.LogInformation("Chat Coordinator Worker running at: {Time}", DateTime.Now);

			using var scope = _services.CreateScope();
			var engine = scope.ServiceProvider.GetRequiredService<IChatCoordinatorEngine>();
			await engine.ProcessNextSessionAsync();
			await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
		}
	}
}
