using Autofac;
using SupportChat.Application.Interfaces.Messaging;
using SupportChat.Application.Interfaces.Repositories;
using SupportChat.Infrastructure.Data;
using SupportChat.Infrastructure.Messaging;
using SupportChat.Infrastructure.Repositories;

namespace SupportChat.Infrastructure.Modules;

public class InfrastructureModule: Module
{
	protected override void Load(ContainerBuilder builder)
	{
		builder.RegisterType<AppDbContext>()
		   .AsSelf()
		   .InstancePerLifetimeScope();

		// Generic repository
		builder.RegisterGeneric(typeof(Repository<>))
			   .As(typeof(IRepository<>))
			   .InstancePerLifetimeScope();

		// Agent repository
		builder.RegisterType<AgentRepository>()
			   .As<IAgentRepository>()
			   .InstancePerLifetimeScope();

		// ChatSession repository
		builder.RegisterType<ChatSessionRepository>()
			   .As<IChatSessionRepository>()
			   .InstancePerLifetimeScope();

		// RabbitMQ publisher
		builder
			.RegisterType<RabbitMqPublisher>()
			.As<IRabbitMqPublisher>()
			.SingleInstance();

		// RabbitMQ consumer
		builder
		  .RegisterType<RabbitMqConsumer>()
		  .As<IRabbitMqConsumer>()
		  .SingleInstance();
	}
}
