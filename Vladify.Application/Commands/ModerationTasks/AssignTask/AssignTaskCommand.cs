using MediatR;

namespace Vladify.Application.Commands.ModerationTasks.AssignTask;

public record AssignTaskCommand(string ModeratorId) : IRequest<Guid?>;
