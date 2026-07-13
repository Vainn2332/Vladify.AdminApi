using Vladify.Application.Models;
using Vladify.Domain.Entities;

namespace Vladify.Application.Interfaces;

public interface IModerationTaskRepository
{
    public Task<ModerationTask> CreateAsync(ModerationTask task, CancellationToken cancellationToken);

    public List<ModerationTask> GetAllAsync(PaginationFilter paginationFilter, CancellationToken cancellationToken);

    public ModerationTask GetAsync(Guid id, CancellationToken cancellationToken);

    public ModerationTask UpdateAsync(ModerationTask task, CancellationToken cancellationToken);

    public ModerationTask DeleteAsync(Guid id, CancellationToken cancellationToken);
}
