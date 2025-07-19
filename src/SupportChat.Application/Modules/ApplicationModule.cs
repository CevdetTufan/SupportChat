using Autofac;
using SupportChat.Application.Commands.AssignSession;
using SupportChat.Application.Commands.CreateChatSession;
using SupportChat.Application.Commands.EndSession;
using SupportChat.Application.Commands.MarkSessionInactive;
using SupportChat.Application.Commands.ProcessNextSession;
using SupportChat.Application.Interfaces.Commands;
using SupportChat.Application.Interfaces.Queries;
using SupportChat.Application.Queries.GetActiveAgents;
using SupportChat.Application.Queries.GetNextUnassignedSession;
using SupportChat.Domain.Agents;
using SupportChat.Domain.ChatSessions;

namespace SupportChat.Application.Modules;

public class ApplicationModule : Module
{
	protected override void Load(ContainerBuilder builder)
	{
		// Queries
		builder.RegisterType<GetNextUnassignedSessionHandler>()
			   .As<IQueryHandler<GetNextUnassignedSessionQuery, ChatSession?>>()
			   .InstancePerLifetimeScope();

		builder.RegisterType<GetActiveAgentsHandler>()
			   .As<IQueryHandler<GetActiveAgentsQuery, IReadOnlyCollection<Agent>>>()
			   .InstancePerLifetimeScope();

		// Commands
		builder.RegisterType<AssignSessionHandler>()
			   .As<ICommandHandler<AssignSessionCommand>>()
			   .InstancePerLifetimeScope();

		builder.RegisterType<ProcessNextSessionHandler>()
			   .As<ICommandHandler<ProcessNextSessionCommand>>()
			   .InstancePerLifetimeScope();

		builder.RegisterType<CreateChatSessionHandler>()
			.As<ICommandHandler<CreateChatSessionCommand>>()
			   .InstancePerLifetimeScope();

		builder.RegisterType<EndSessionHandler>()
			   .As<ICommandHandler<EndSessionCommand>>()
			   .InstancePerLifetimeScope();

		builder.RegisterType<MarkSessionInactiveHandler>()
			.As<ICommandHandler<MarkSessionInactiveCommand>>()
			   .InstancePerLifetimeScope();
	}
}
