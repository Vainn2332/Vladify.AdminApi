using Grpc.Core;
using MediatR;
using Vladify.Application.Commands.ModerationTasks.CreateTask;
using Vladify.GrpcContracts;

namespace Vladify.AdminApi.Grpc.ModerationTask;

public class ModerationGrpcService(IMediator mediator) : ModerationGrpc.ModerationGrpcBase
{
    public async override Task<CreateTaskResponse> CreateTask(CreateTaskRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.SongId, out var songId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid guid!"));
        }
        var command = new CreateTaskCommand(songId);

        var response = await mediator.Send(command, context.CancellationToken);

        return new CreateTaskResponse()
        {
            TaskId = response.Id.ToString()
        };
    }
}
