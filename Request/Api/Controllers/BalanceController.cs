namespace Request.Api.Controllers
{

    using Microsoft.AspNetCore.Mvc;
    using Request.Application.DTOs.Request;
    using Request.Application.Interfaces;

    [Route("api/[controller]")]
    [ApiController]
    public class BalanceController(
        IBalanceService balanceService
    ) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetBalanceQuery? query)
        {
            var balances = await balanceService.GetAllAsync(query);

            return Ok(balances);
        }

        [HttpPost("createBalance")]
        public async Task<IActionResult> CreateBalance([FromBody] CreateBalanceRequest newBalance)
        {
            var successResponse = await balanceService.CreateBalance(newBalance);

            if (!successResponse.Success) return BadRequest(successResponse.Message);

            return Ok(successResponse);
        }

        [HttpPut("updateBalance")]
        public async Task<IActionResult> UpdateBalance([FromBody] UpdateBalanceRequest updateBalance)
        {
            var successResponse = await balanceService.UpdateBalance(updateBalance);

            if (!successResponse.Success) return BadRequest(successResponse.Message);

            return Ok(successResponse);
        }

        [HttpDelete("deleteBalance/{userId}&{type}&{year}")]
        public async Task<IActionResult> DeleteBalance([FromRoute] int userId, [FromRoute] byte type, [FromRoute] int year)
        {
            var successResponse = await balanceService.DeleteBalance(userId, type, year);

            if (!successResponse.Success) return BadRequest(successResponse.Message);

            return Ok(successResponse);
        }
    }
}