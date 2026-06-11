using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.BackendServer.Helpers;
using WarehouseManagement.BackendServer.Services.Interfaces;
using WarehouseManagement.ViewModels.Systems.Authentication;
using WarehouseManagement.ViewModels.Systems.Login;

namespace WarehouseManagement.BackendServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IJwtService _jwtService;

        public AuthenticationController(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponseModel>> Login(LoginRequestModel request)
        {
            // Hard-code tài khoản test
            if (request.UserName == "admin123" &&
                request.Password == "Admin@123")
            {
                return Ok(new LoginResponseModel
                {
                    AccessToken = "fake-access-token",
                    RefreshToken = "fake-refresh-token"
                });
            }

            var result = await _jwtService.Authenticate(request);

            if (result is null)
                return Unauthorized();

            return result;
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<ActionResult<LoginResponseModel>> Register(RegisterRequestModel request)
        {
            var result = await _jwtService.Register(request);
            if (result is null)
                return BadRequest("Registration failed");

            return result;
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (request is null || string.IsNullOrEmpty(request.AccessToken) || string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest("Invalid data");
            }

            var result = await _jwtService.RefreshToken(request.AccessToken, request.RefreshToken);

            if (result == null)
            {
                return Unauthorized("Time out");
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            var success = await _jwtService.ChangePassword(User.Identity!.Name!, request);
            return success ? Ok() : BadRequest("Failed to update password in user");
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _jwtService.RevokeToken(User.Identity!.Name!);
            return NoContent();
        }

        [Authorize]
        [HttpGet("token-status")]
        public async Task<IActionResult> GetTokenStatus()
        {
            // Lấy Access Token từ Header
            string accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var accessRemaining = _jwtService.GetAccessTokenRemainingTime(accessToken);
            var test = User.Identity!.Name!;
            var refreshRemaining = await _jwtService.GetRefreshTokenRemainingTime(User.Identity!.Name!);

            return Ok(new ApiOkResponse<object>(new
            {
                AccessTokenRemainingMinutes = Math.Round(accessRemaining.TotalMinutes, 2),
                RefreshTokenRemainingDays = refreshRemaining.HasValue
                    ? Math.Round(refreshRemaining.Value.TotalDays, 2)
                    : 0,
                IsNearExpiry = accessRemaining.TotalMinutes < 5 
            }));
        }
    }
}
