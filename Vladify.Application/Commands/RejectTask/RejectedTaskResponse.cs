namespace Vladify.Application.Commands.RejectTask;

public record RejectedTaskResponse(Guid Id, DateTimeOffset ResolvedAt);
