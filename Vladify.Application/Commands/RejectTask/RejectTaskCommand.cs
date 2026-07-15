using MediatR;

namespace Vladify.Application.Commands.RejectTask;

public record RejectTaskCommand(Guid TaskId, Guid ModeratorId, string RejectionReason) : IRequest<RejectedTaskResponse>;