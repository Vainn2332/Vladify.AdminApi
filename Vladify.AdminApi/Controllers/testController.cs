using MediatR;
using Microsoft.AspNetCore.Mvc;
using Vladify.Application.Commands.ModerationTasks.ApproveTask;
using Vladify.Application.Commands.ModerationTasks.AssignTask;
using Vladify.Application.Commands.ModerationTasks.CreateTask;
using Vladify.Application.Commands.ModerationTasks.RejectTask;

namespace Vladify.AdminApi.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class testController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public Task<CreatedTaskResponse> Create(CreateTaskCommand createTaskCommand)
        {
            return mediator.Send(createTaskCommand);
        }

        [HttpPost("AssignTask")]
        public Task<Guid?> Asssgn(AssignTaskCommand assignTaskCommand)
        {
            return mediator.Send(assignTaskCommand);
        }


        [HttpPut("Approve")]
        public Task<ApprovedTaskResponse> Approve(ApproveTaskCommand approveTaskCommand)
        {
            return mediator.Send(approveTaskCommand);
        }

        [HttpPut("Reject")]
        public Task<RejectedTaskResponse> Reject(RejectTaskCommand rejectTaskCommand)
        {
            return mediator.Send(rejectTaskCommand);
        }
    }
}
