namespace Vladify.Application.Commands.ModerationTask.RejectTask;

public record RejectedTaskResponse(Guid Id, DateTimeOffset ResolvedAt);
