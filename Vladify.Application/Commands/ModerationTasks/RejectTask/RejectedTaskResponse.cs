namespace Vladify.Application.Commands.ModerationTasks.RejectTask;

public record RejectedTaskResponse(Guid Id, DateTimeOffset ResolvedAt, string Message);
