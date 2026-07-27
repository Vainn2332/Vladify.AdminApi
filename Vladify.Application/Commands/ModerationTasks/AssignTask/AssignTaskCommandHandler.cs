using MediatR;
using Vladify.Application.Constants;
using Vladify.Application.Exceptions;
using Vladify.Application.Interfaces;

namespace Vladify.Application.Commands.ModerationTasks.AssignTask;

public class AssignTaskCommandHandler(IModerationTaskRepository repository) : IRequestHandler<AssignTaskCommand, Guid?>
{
    public async Task<Guid?> Handle(AssignTaskCommand request, CancellationToken cancellationToken)
    {
        var hasActiveTask = await repository.HasActiveTaskAsync(request.ModeratorId, cancellationToken);
        if (hasActiveTask)
        {
            throw new AlreadyHasActiveTaskException(ErrorMessages.AlreadyHasActiveTask);
        }

        return await repository.ClaimNextPendingTaskAsync(request.ModeratorId, cancellationToken);
    }
}
