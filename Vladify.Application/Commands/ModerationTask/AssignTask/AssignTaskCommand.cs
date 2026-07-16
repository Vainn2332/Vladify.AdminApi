using MediatR;

namespace Vladify.Application.Commands.ModerationTask.AssignTask;

public record AssignTaskCommand(Guid ModeratorId) : IRequest<Guid?>;
