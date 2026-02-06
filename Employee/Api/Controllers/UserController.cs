namespace Employee.Controllers
{
    using Employee.Application.DTOs;
    using Employee.Application.DTOs.Request;
    using Employee.Application.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class UserController(
        IUserService userService
    ) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetUserRequest? request)
        {
            var users = await userService.GetAllAsync(request);
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await userService.GetByIdAsync(id);
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest dto)
        {
            var (isSuccess, message, createdRequest) = await userService.CreateAsync(dto);

            if (!isSuccess) return BadRequest(message);

            return Ok(createdRequest);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] UpdateUserRequest dto)
        {
            var (isSuccess, message, updatedRequest) = await userService.UpdateAsync(dto);

            if (!isSuccess) return BadRequest(message);

            return Ok(updatedRequest);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var (isSuccess, message, deleted) = await userService.DeleteAsync(id);

            if (!isSuccess) return BadRequest(message);

            return Ok(deleted);
        }

        [HttpPut("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(int id)
        {
            var (isSuccess, message, newPassword) = await userService.ResetPasswordAsync(id);

            if (!isSuccess) return BadRequest(message);

            return Ok(newPassword);
        }

        // [HttpPut("{id}/increment-failed-login")]
        // public async Task<IActionResult> IncrementFailedLoginCount(int id)
        // {
        //     var (isSuccess, message, failedLoginCount) = await userService.IncrementFailedLoginCountAsync(id);

        //     if (!isSuccess) return BadRequest(message);

        //     return Ok(failedLoginCount);
        // }

        [HttpPut("{id}/lock-user")]
        public async Task<IActionResult> LockUser(int id)
        {
            var (isSuccess, message, locked) = await userService.LockUserAsync(id);

            if (!isSuccess) return BadRequest(message);

            return Ok(locked);
        }
    }
}