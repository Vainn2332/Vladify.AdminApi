using MediatR;

namespace Vladify.Application.Commands.ModerationTask.RejectTask;

public record RejectTaskCommand(Guid TaskId, Guid ModeratorId, string RejectionReason) : IRequest<RejectedTaskResponse>;