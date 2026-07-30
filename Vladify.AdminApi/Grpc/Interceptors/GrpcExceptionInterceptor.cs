using Grpc.Core;
using Grpc.Core.Interceptors;
using Vladify.Application.Exceptions;

namespace Vladify.AdminApi.Grpc.Interceptors;

public class GrpcExceptionInterceptor(ILogger<GrpcExceptionInterceptor> logger) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled gRPC error has occured!");
            throw HandleError(ex);
        }
    }

    private static RpcException HandleError(Exception ex)
    {
        if (ex is RpcException rpcException)
        {
            return rpcException;
        }

        var status = ex switch
        {
            NotFoundException => new Status(StatusCode.NotFound, ex.Message),
            TaskAssignedToDifferentModeratorException => new Status(StatusCode.InvalidArgument, ex.Message),
            TaskNotClaimedException => new Status(StatusCode.InvalidArgument, ex.Message),
            _ => new Status(StatusCode.Internal, "An internal server error occurred.")
        };

        return new RpcException(status);
    }
}
