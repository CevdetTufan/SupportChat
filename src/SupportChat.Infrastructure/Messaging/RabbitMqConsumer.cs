using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SupportChat.Application.Interfaces.Messaging;
using System.Text;
using System.Text.Json;

namespace SupportChat.Infrastructure.Messaging;
internal class RabbitMqConsumer : IRabbitMqConsumer, IDisposable
{
	private readonly IChannel _channel;
	private readonly IConnection _connection;

	private bool _disposed;

	public RabbitMqConsumer(IOptions<RabbitMqSettings> options)
	{
		var settings = options.Value;
		var factory = new ConnectionFactory
		{
			HostName = settings.Host,
			Port = settings.Port,
			UserName = settings.UserName,
			Password = settings.Password,
			VirtualHost = settings.VirtualHost,
			Ssl = new SslOption { Enabled = settings.UseSsl }
		};

		_connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
		_channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
	}

	public async Task SubscribeAsync<T>(string queueName, Func<T, Task> onMessage)
	{
		await _channel.QueueDeclareAsync(
			queue: queueName,
			durable: true,
			exclusive: false,
			autoDelete: false);

		var consumer = new AsyncEventingBasicConsumer(_channel);

		consumer.ReceivedAsync += async (_, ea) =>
		{
			try
			{
				var json = Encoding.UTF8.GetString(ea.Body.Span);
				var message = JsonSerializer.Deserialize<T>(json);
				if (!EqualityComparer<T>.Default.Equals(message, default(T)) && message is not null)
				{
					await onMessage(message);
				}

				await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
			}
			catch
			{
				await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
			}
		};

		await _channel.BasicConsumeAsync(
			queue: queueName,
			autoAck: true,
			consumerTag:
			string.Empty,
			noLocal: false,
			exclusive: false,
			arguments: null,
			consumer: consumer);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposed)
		{
			if (disposing)
			{
				_channel?.Dispose();
				_connection?.Dispose();
			}

			_disposed = true;
		}
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}
