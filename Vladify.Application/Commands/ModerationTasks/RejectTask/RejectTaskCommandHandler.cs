using AutoMapper;
using MediatR;
using Vladify.Application.Constants;
using Vladify.Application.Exceptions;
using Vladify.Application.Interfaces;
using Vladify.Domain.Enums;

namespace Vladify.Application.Commands.ModerationTasks.RejectTask;

public class RejectTaskCommandHandler(IModerationTaskRepository repository, IMapper mapper) : IRequestHandler<RejectTaskCommand, RejectedTaskResponse>
{
    public async Task<RejectedTaskResponse> Handle(RejectTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await repository.GetAsync(request.TaskId, cancellationToken)
           ?? throw new NotFoundException(ErrorMessages.TaskNotFoundById);
        if (task.AssignedModeratorId is null)
        {
            throw new TaskNotClaimedException(ErrorMessages.TaskNotClaimed);
        }
        if (task.AssignedModeratorId != request.ModeratorId)
        {
            throw new TaskAssignedToDifferentModeratorException(ErrorMessages.TaskAssignedToDifferentModerator);
        }

        task.Status = ModerationStatus.Rejected;
        task.ResolvedAt = DateTime.UtcNow;
        task.Message = request.RejectionReason;

        var updatedTask = await repository.UpdateAsync(task, cancellationToken);

        return mapper.Map<RejectedTaskResponse>(updatedTask);
    }
}
