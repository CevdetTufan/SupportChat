using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SupportChat.Application.Interfaces.Coordination;
using SupportChat.Domain.ChatSessions.Exceptions;

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

			try
			{
				await engine.ProcessNextSessionAsync();
			}
			catch (SessionAlreadyAssignedException ex)	
			{
				_logger.LogDebug(ex, ex.Message);
			}
			catch (SessionAlreadyEndedException ex)
			{
				_logger.LogDebug(ex, ex.Message);
			}
			catch (SessionPollCannotInactiveException ex)
			{
				_logger.LogDebug(ex, ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "ChatCoordinatorWorker eroror: {Message}", ex.Message);

				throw;
			}

			await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
		}
	}
}
