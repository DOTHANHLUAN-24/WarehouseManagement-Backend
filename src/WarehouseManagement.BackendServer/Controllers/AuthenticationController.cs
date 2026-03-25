using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.BackendServer.Services;
using WarehouseManagement.ViewModels.Systems.Authentication;
using WarehouseManagement.ViewModels.Systems.Login;

namespace WarehouseManagement.BackendServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly JwtService _jwtService;

        public AuthenticationController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponseModel>> Login(LoginRequestModel request)
        {
            var result = await _jwtService.Authenticate(request);
            if (result is null)
                return Unauthorized();

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
    }
}
