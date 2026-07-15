using MediatR;

namespace Vladify.Application.Commands.AssignTask;

public record AssignTaskCommand(Guid TaskId, Guid ModeratorId) : IRequest;
