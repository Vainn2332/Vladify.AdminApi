using MediatR;
using Vladify.Application.Interfaces;

namespace Vladify.Application.Commands.ModerationTasks.AssignTask;

public class AssignTaskCommandHandler(IModerationTaskRepository repository) : IRequestHandler<AssignTaskCommand, Guid?>
{
    public Task<Guid?> Handle(AssignTaskCommand request, CancellationToken cancellationToken)
    {
        return repository.ClaimNextPendingTaskAsync(request.ModeratorId, cancellationToken);
    }
}
