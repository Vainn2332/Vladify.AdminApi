using MediatR;

namespace Vladify.Application.Commands.ModerationTasks.CreateTask;

public record CreateTaskCommand(Guid SongId) : IRequest<CreatedTaskResponse>;
