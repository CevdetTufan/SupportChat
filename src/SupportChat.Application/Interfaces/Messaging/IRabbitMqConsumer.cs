namespace SupportChat.Application.Interfaces.Messaging;

public interface IRabbitMqConsumer
{
	Task SubscribeAsync<T>(string queueName, Func<T, Task> onMessage);
}
