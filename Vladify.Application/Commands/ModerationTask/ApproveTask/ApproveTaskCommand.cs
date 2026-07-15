using MediatR;

namespace Vladify.Application.Commands.ModerationTask.ApproveTask;

public record ApproveTaskCommand(Guid TaskId, Guid ModeratorId) : IRequest<ApprovedTaskResponse>;
