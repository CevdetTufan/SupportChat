using Autofac;
using SupportChat.Application.Commands.Agents;
using SupportChat.Application.Commands.ChatSessions;
using SupportChat.Application.Interfaces.Commands;
using SupportChat.Application.Interfaces.Coordination;
using SupportChat.Application.Interfaces.Queries;
using SupportChat.Application.Queries.Agents;
using SupportChat.Application.Queries.ChatSessions;
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
	}
}
