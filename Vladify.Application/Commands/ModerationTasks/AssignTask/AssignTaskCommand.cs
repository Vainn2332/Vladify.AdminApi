using MediatR;

namespace Vladify.Application.Commands.ModerationTasks.AssignTask;

public record AssignTaskCommand(Guid ModeratorId) : IRequest<Guid?>;
