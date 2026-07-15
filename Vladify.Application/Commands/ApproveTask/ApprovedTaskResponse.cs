namespace Vladify.Application.Commands.ApproveTask;

public record ApprovedTaskResponse(Guid Id, DateTimeOffset ResolvedAt);
