using Autofac;
using Microsoft.Extensions.Hosting;
using SupportChat.Application.Interfaces.Coordination;
using SupportChat.Application.Interfaces.Messaging;
using SupportChat.Application.Interfaces.Persistence;
using SupportChat.Application.Interfaces.Repositories;
using SupportChat.Infrastructure.Coordination;
using SupportChat.Infrastructure.Data;
using SupportChat.Infrastructure.Messaging;
using SupportChat.Infrastructure.Repositories;
using SupportChat.Infrastructure.Workers;

namespace SupportChat.Infrastructure.Modules;

public class InfrastructureModule: Module
{
	protected override void Load(ContainerBuilder builder)
	{
		builder.RegisterType<AppDbContext>()
		   .AsSelf()
		   .InstancePerLifetimeScope();

		// UnitOfWork
		builder
		  .RegisterType<UnitOfWork>()
		  .As<IUnitOfWork>()
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

		//Background service hosted service
		builder
			.RegisterType<ChatCoordinatorWorker>()
			.As<IHostedService>()
			.SingleInstance();

		builder.RegisterType<PollingMonitorWorker>()
			.As<IHostedService>()
			.SingleInstance();

		//strategy
		builder
			.RegisterType<SeniorityBasedAssignment>()
			.As<ISessionAssignmentStrategy>()
			.SingleInstance();

		builder
			.RegisterType<AgentSelector>()
			.As<IAgentSelector>()
			.InstancePerLifetimeScope();

		builder
			.RegisterType<JuniorOverflowHandler>()
			.As<IOverflowHandler>()
			.InstancePerLifetimeScope();

		// ChatCoordinatorEngine
		builder
			.RegisterType<ChatCoordinatorEngine>()
			.As<IChatCoordinatorEngine>()
			.InstancePerLifetimeScope();
	}
}
