namespace Auth.Api.Controllers
{
    using Auth.Api.ValueObjects;
    using Auth.Application.DTOs;
    using Auth.Application.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Shared.Notifications.Teams;

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly INotifier notifier;

        private readonly IAuthService authService;

        public AuthController(INotifier _notifier, IAuthService _authService)
        {
            notifier = _notifier;
            authService = _authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var result = await authService.Login(loginRequest);

            SetRefreshTokenCookie(result.UserResult.RefreshToken);

            if (string.IsNullOrEmpty(result.errorMessage))
            {
                var res = new LoginResponse
                {
                    AccessToken = result.UserResult.AccessToken,
                    User = new UserResponse
                    {
                        Id = result.UserResult.UserId.ToString(),
                        Name = result.UserResult.Username,
                        Roles = result.UserResult.Roles
                    },
                    IsRequiredChangePW = result.UserResult.requiredChangePW ?? false
                };

                return Ok(res);
            }

            return BadRequest(result.errorMessage);
        }

        // [HttpPost("register")]
        // [Authorize]
        // public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        // {
        //     var result = await authService.Register(registerRequest);

        //     if (result.registerSuccess)
        //         return Ok("Register success");

        //     return BadRequest(result.errorMessage);
        // }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Get refresh token from cookie
            var refreshToken = Request.Cookies["refresh_token"];

            // Revoke refresh token from DB
            if (!string.IsNullOrEmpty(refreshToken))
                try
                {
                    await authService.RevokeRefreshToken(refreshToken);
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }

            DeleteRefreshTokenCookie("refresh_token");

            return NoContent();
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateRefreshToken()
        {
            // Get refresh token from cookie
            var refreshToken = Request.Cookies["refresh_token"];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized();

            try
            {
                var result = await authService.UpdateRefreshToken(refreshToken);

                if (result == null)
                    return Unauthorized();

                SetRefreshTokenCookie(result.UserResult.RefreshToken);

                var res = new LoginResponse
                {
                    AccessToken = result.UserResult.AccessToken,
                    User = new UserResponse
                    {
                        Id = result.UserResult.UserId.ToString(),
                        Name = result.UserResult.Username,
                        Roles = result.UserResult.Roles
                    }
                };
                return Ok(res);
            }
            catch
            {
                // Treat any refresh failure as unauthorized to avoid 500 responses
                return Unauthorized();
            }
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var options = new CookieOptions
            {
                HttpOnly = true,
                // Secure = true,
                Secure = Request.IsHttps, // only secure over HTTPS
                SameSite = Request.IsHttps ? SameSiteMode.None : SameSiteMode.Lax,
                // SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7),
                Path = "/"
            };

            Response.Cookies.Append("refresh_token", refreshToken, options);
        }

        private void DeleteRefreshTokenCookie(string cookieName)
        {
            Response.Cookies.Delete(cookieName, new CookieOptions
            {
                HttpOnly = true,
                // Secure = true,
                Secure = Request.IsHttps, // only secure over HTTPS
                SameSite = Request.IsHttps ? SameSiteMode.None : SameSiteMode.Lax,
                // SameSite = SameSiteMode.None,
                Path = "/",
            });
        }
    }
}