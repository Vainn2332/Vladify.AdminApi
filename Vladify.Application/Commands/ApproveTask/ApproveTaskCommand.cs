using MediatR;

namespace Vladify.Application.Commands.ApproveTask;

public record ApproveTaskCommand(Guid TaskId, Guid ModeratorId) : IRequest<ApprovedTaskResponse>;
