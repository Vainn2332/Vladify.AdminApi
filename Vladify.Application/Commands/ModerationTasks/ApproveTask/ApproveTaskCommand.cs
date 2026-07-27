using MediatR;

namespace Vladify.Application.Commands.ModerationTasks.ApproveTask;

public record ApproveTaskCommand(Guid TaskId, string ModeratorId) : IRequest<ApprovedTaskResponse>;
