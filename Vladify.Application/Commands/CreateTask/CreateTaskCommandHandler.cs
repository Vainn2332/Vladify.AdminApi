using AutoMapper;
using MediatR;
using Vladify.Application.Interfaces;
using Vladify.Domain.Entities;
using Vladify.Domain.Enums;

namespace Vladify.Application.Commands.CreateTask;

public class CreateTaskCommandHandler(IModerationTaskRepository repository, IMapper mapper) : IRequestHandler<CreateTaskCommand, CreatedTaskResponse>
{
    public async Task<CreatedTaskResponse> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = mapper.Map<ModerationTask>(request);
        task.CreatedAt = DateTime.UtcNow;
        task.Status = ModerationStatus.Pending;

        var createdTask = await repository.CreateAsync(task, cancellationToken);

        return mapper.Map<CreatedTaskResponse>(createdTask);
    }
}
