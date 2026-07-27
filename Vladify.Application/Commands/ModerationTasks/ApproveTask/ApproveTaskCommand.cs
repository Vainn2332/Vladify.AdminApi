using MediatR;

namespace Vladify.Application.Commands.ModerationTasks.ApproveTask;

public record ApproveTaskCommand(Guid TaskId, Guid ModeratorId) : IRequest<ApprovedTaskResponse>;
