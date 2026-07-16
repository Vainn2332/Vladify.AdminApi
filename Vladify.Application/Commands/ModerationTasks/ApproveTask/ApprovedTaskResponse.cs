namespace Vladify.Application.Commands.ModerationTasks.ApproveTask;

public record ApprovedTaskResponse(Guid Id, DateTimeOffset ResolvedAt);
