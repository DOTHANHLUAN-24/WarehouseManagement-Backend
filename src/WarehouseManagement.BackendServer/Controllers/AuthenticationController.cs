using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.BackendServer.Services;
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
    }
}
