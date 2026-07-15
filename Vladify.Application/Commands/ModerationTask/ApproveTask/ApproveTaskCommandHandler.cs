using AutoMapper;
using MediatR;
using Vladify.Application.Constants;
using Vladify.Application.Exceptions;
using Vladify.Application.Interfaces;
using Vladify.Domain.Enums;

namespace Vladify.Application.Commands.ModerationTask.ApproveTask;

public class ApproveTaskCommandHandler(IModerationTaskRepository repository, IMapper mapper) : IRequestHandler<ApproveTaskCommand, ApprovedTaskResponse>
{
    public async Task<ApprovedTaskResponse> Handle(ApproveTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await repository.GetAsync(request.TaskId, cancellationToken)
            ?? throw new NotFoundException(ErrorMessages.TaskNotFoundById);
        if (task.AssignedModeratorId != request.ModeratorId)
        {
            throw new TaskAssignedToDifferentModeratorException(ErrorMessages.TaskAssignedToDifferentModerator);
        }

        task.Status = ModerationStatus.Approved;
        task.ResolvedAt = DateTime.UtcNow;

        var updatedTask = await repository.UpdateAsync(task, cancellationToken);

        return mapper.Map<ApprovedTaskResponse>(updatedTask);
    }
}
