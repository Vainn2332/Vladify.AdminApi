using MediatR;

namespace Vladify.Application.Commands.ModerationTask.CreateTask;

public record CreateTaskCommand(Guid SongId) : IRequest<CreatedTaskResponse>;
