namespace SupportChat.Application.Interfaces.Messaging;

public interface IRabbitMqPublisher
{
	Task PublishAsync<T>(T message, string queueName);
}
