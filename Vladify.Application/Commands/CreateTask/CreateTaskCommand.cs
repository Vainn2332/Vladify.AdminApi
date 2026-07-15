using MediatR;

namespace Vladify.Application.Commands.CreateTask;

public record CreateTaskCommand(Guid SongId) : IRequest<CreatedTaskResponse>;
