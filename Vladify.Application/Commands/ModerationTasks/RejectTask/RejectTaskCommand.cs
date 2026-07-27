using MediatR;

namespace Vladify.Application.Commands.ModerationTasks.RejectTask;

public record RejectTaskCommand(Guid TaskId, string ModeratorId, string RejectionReason) : IRequest<RejectedTaskResponse>;
