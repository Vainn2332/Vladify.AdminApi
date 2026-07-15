using MediatR;
using Vladify.Application.Constants;
using Vladify.Application.Exceptions;
using Vladify.Application.Interfaces;

namespace Vladify.Application.Commands.ModerationTask.AssignTask;

public class AssignTaskCommandHandler(IModerationTaskRepository repository) : IRequestHandler<AssignTaskCommand>
{
    public async Task Handle(AssignTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await repository.GetAsync(request.TaskId, cancellationToken)
            ?? throw new NotFoundException(ErrorMessages.TaskNotFoundById);
        if (task.AssignedModeratorId != request.ModeratorId)
        {
            throw new TaskAssignedToDifferentModeratorException(ErrorMessages.TaskAssignedToDifferentModerator);
        }

        task.AssignedModeratorId = request.ModeratorId;

        await repository.UpdateAsync(task, cancellationToken);
    }
}
