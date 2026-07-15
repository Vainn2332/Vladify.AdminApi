namespace Vladify.Application.Commands.ApproveTask;

public record ApprovedTaskResponse
{
    public Guid Id { get; set; }

    public DateTimeOffset? ResolvedAt { get; set; }
}
