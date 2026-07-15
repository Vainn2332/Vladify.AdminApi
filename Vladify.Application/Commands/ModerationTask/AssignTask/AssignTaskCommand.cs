using MediatR;

namespace Vladify.Application.Commands.ModerationTask.AssignTask;

public record AssignTaskCommand(Guid TaskId, Guid ModeratorId) : IRequest;
