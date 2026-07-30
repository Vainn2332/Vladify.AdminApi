using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vladify.AdminApi.Constants;
using Vladify.AdminApi.Extensions;
using Vladify.Application.Commands.ModerationTasks.ApproveTask;
using Vladify.Application.Commands.ModerationTasks.AssignTask;
using Vladify.Application.Commands.ModerationTasks.RejectTask;

namespace Vladify.AdminApi.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    [Authorize(Policy = AuthPolicies.AdminOrModerator)]
    public class TasksController(IMediator mediator) : ControllerBase
    {
        [HttpPost("assign")]
        public Task<Guid?> AsssignTask()
        {
            var moderatorId = User.GetAuth0Id();

            var command = new AssignTaskCommand(moderatorId);

            return mediator.Send(command);
        }

        [HttpPut("{id}/approve")]
        public Task<ApprovedTaskResponse> Approve(Guid id)
        {
            var moderatorId = User.GetAuth0Id();

            var command = new ApproveTaskCommand(id, moderatorId);

            return mediator.Send(command);
        }

        [HttpPut("{id}/reject")]
        public Task<RejectedTaskResponse> Reject(Guid id, string rejactionReason)
        {
            var moderatorId = User.GetAuth0Id();

            var command = new RejectTaskCommand(id, moderatorId, rejactionReason);

            return mediator.Send(command);
        }
    }
}
