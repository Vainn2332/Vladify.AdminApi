using Vladify.Domain.Entities;

namespace Vladify.Application.Interfaces;

public interface IModerationTaskRepository
{
    public Task<ModerationTask> CreateAsync(ModerationTask task, CancellationToken cancellationToken);

    public Task<Guid?> ClaimNextPendingTaskAsync(Guid moderatorId, CancellationToken cancellationToken);

    public Task<ModerationTask?> GetAsync(Guid id, CancellationToken cancellationToken);

    public Task<ModerationTask> UpdateAsync(ModerationTask task, CancellationToken cancellationToken);

    public Task DeleteAsync(ModerationTask task, CancellationToken cancellationToken);
}
