using SupportChat.Application.Interfaces.Queries;
using SupportChat.Application.Interfaces.Repositories;
using SupportChat.Domain.Agents;

namespace SupportChat.Application.Queries.Agents;

internal class GetActiveAgentsHandler : IQueryHandler<GetActiveAgentsQuery, IReadOnlyCollection<Agent>>
{
	private readonly IAgentRepository _agentRepository;

	public GetActiveAgentsHandler(IAgentRepository agentRepository)
	{
		_agentRepository = agentRepository;
	}

	public async Task<IReadOnlyCollection<Agent>> HandleAsync(GetActiveAgentsQuery query)
	{
		return await _agentRepository.GetActiveAgentsAsync();
	}
}
