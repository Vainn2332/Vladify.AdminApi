using Vladify.Domain.Enums;

namespace Vladify.Application.Commands.ModerationTask.CreateTask;

public record CreatedTaskResponse
{
    public Guid Id { get; set; }

    public Guid SongId { get; set; }

    public ModerationStatus Status { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
