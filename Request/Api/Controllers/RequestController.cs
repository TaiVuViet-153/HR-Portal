namespace Request.Api.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Request.Application.DTOs;
    using Request.Application.Interfaces;
    using Shared.Notifications.Teams;

    [Route("api/[controller]")]
    [ApiController]
    public class RequestController(
        IRequestService requestService,
        IBalanceService balanceService,
        INotifier notifier
    )
    : ControllerBase
    {
        [HttpGet("filteredRequests")]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery] GetRequestQuery? query)
        {
            var res = await requestService.GetRequests(query ?? new GetRequestQuery());

            return Ok(res);
        }
        [HttpPost("createRequest")]
        [Authorize]
        public async Task<IActionResult> CreateRequest([FromBody] CreateRequest newRequest)
        {
            var (createSuccess, message, createdRequest) = await requestService.CreateRequest(newRequest);

            if (!createSuccess) return BadRequest(message);

            return Ok(createdRequest);
        }
        [HttpPut("updateRequest")]
        [Authorize]
        public async Task<IActionResult> UpdateRequest([FromBody] UpdateRequest updateRequest)
        {
            var (updateSuccess, message, updatedRequest) = await requestService.UpdateRequest(updateRequest);

            if (!updateSuccess) return BadRequest(message);

            return Ok(updatedRequest);
        }
        [HttpPatch("deleteRequest/{requestId}")]
        [Authorize]
        public async Task<IActionResult> DeleteRequest([FromRoute] int requestId)
        {
            var (deleteSuccess, message, deletedRequest) = await requestService.DeleteRequest(requestId);

            if (!deleteSuccess) return BadRequest(message);

            return Ok(deleteSuccess);
        }

        [HttpPost("createBalance")]
        [Authorize]
        public async Task<IActionResult> CreateBalance([FromBody] CreateBalance newBalance)
        {
            var (createSuccess, message) = await balanceService.CreateBalance(newBalance);

            if (!createSuccess) return BadRequest(message);

            return Ok(message);
        }
    }
}