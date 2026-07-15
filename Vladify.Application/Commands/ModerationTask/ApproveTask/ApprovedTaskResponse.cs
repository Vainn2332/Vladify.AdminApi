namespace Vladify.Application.Commands.ModerationTask.ApproveTask;

public record ApprovedTaskResponse(Guid Id, DateTimeOffset ResolvedAt);
