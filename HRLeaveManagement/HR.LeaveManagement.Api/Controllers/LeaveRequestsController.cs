using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HR.LeaveManagement.Application.DTOs.LeaveRequest;
using HR.LeaveManagement.Application.Features.LeaveRequests.Requests.Commands;
using HR.LeaveManagement.Application.Features.LeaveRequests.Requests.Queries;
using HR.LeaveManagement.Application.Responses;
using MediatR;

namespace HR.LeaveManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LeaveRequestsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<LeaveRequestDto>>> Get()
        {
            var leaveRequest = await _mediator.Send(new GetLeaveRequestListRequest());
            return Ok(leaveRequest);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveRequestDto>> Get(int id)
        {
            var leaveRequest = await _mediator.Send(new GetLeaveRequestDetailRequest { Id = id });
            return Ok(leaveRequest);
        }

        [HttpPost]
        public async Task<ActionResult<BaseCommandResponse>> Post([FromBody] CreateLeaveRequestDto leaveRequest)
        {
            var command = new CreateLeaveRequestCommand{ LeaveRequestDto = leaveRequest};
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] UpdateLeaveRequestDto leaveRequest)
        {
            var command = new UpdateLeaveRequestCommand { Id = id, LeaveRequestDto = leaveRequest};
            await _mediator.Send(command);
            return NoContent();
        }

        // PUT api/<LeaveRequestsController>/changeapproval
        [HttpPut("changeapproval")]
        public async Task<ActionResult> ChangeApproval(int id,
            [FromBody] ChangeLeaveRequestApprovalDto changeLeaveRequestApproval)
        {
            var command = new UpdateLeaveRequestCommand {Id = id, ChangeLeaveRequestApprovalDto = changeLeaveRequestApproval};
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var command = new DeleteLeaveRequestCommand() { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
