using Vladify.Application.Models;
using Vladify.Domain.Entities;

namespace Vladify.Application.Interfaces;

public interface IModerationTaskRepository
{
    public Task<ModerationTask> CreateAsync(ModerationTask task, CancellationToken cancellationToken);

    public Task<List<ModerationTask>> GetAllAsync(PaginationFilter paginationFilter, CancellationToken cancellationToken);

    public Task<ModerationTask> GetAsync(Guid id, CancellationToken cancellationToken);

    public Task<ModerationTask> UpdateAsync(ModerationTask task, CancellationToken cancellationToken);

    public Task<ModerationTask> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
