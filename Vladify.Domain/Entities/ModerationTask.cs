using Vladify.Domain.Enums;

namespace Vladify.Domain.Entities;

public class ModerationTask
{
    public Guid Id { get; set; }

    public Guid SongId { get; set; }

    public Guid AssignedModeratorId { get; set; }

    public Status Status { get; set; }

    public string? Message { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? ResolvedAt { get; set; }
}
