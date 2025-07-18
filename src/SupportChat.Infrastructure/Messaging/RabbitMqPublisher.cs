using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SupportChat.Application.Interfaces.Messaging;
using System.Text;
using System.Text.Json;

namespace SupportChat.Infrastructure.Messaging;

internal class RabbitMqPublisher : IRabbitMqPublisher, IDisposable
{
	private readonly IChannel _channel;
	private readonly IConnection _connection;

	private bool _disposed;

	public RabbitMqPublisher(IOptions<RabbitMqSettings> options)
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

	public async Task PublishAsync<T>(T message, string queueName)
	{
		await _channel.QueueDeclareAsync(
			queue: queueName,
			durable: true,
			exclusive: false,
			autoDelete: false);

		var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

		var properties = new BasicProperties
		{
			Persistent = true 
		};

		await _channel.BasicPublishAsync(
			exchange: "",
			routingKey: queueName,
			mandatory: false,
			basicProperties: properties,   
			body: body);
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
