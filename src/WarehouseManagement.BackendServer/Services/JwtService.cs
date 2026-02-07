using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using WarehouseManagement.BackendServer.Data;
using WarehouseManagement.BackendServer.Data.Entities;
using WarehouseManagement.ViewModels.Systems.Login;

namespace WarehouseManagement.BackendServer.Services
{
    public class JwtService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public JwtService(
            ApplicationDbContext context,
            UserManager<User> userManager,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<LoginResponseModel?> Authenticate(LoginRequestModel request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName) ||
                string.IsNullOrWhiteSpace(request.Password))
                return null;

            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
                return null;

            var isValidPassword = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isValidPassword)
                return null;

            var issuer = _configuration["JwtConfig:Issuer"];
            var audience = _configuration["JwtConfig:Audience"];
            var key = _configuration["JwtConfig:Key"];
            var tokenValidityMins = _configuration.GetValue<int>("JwtConfig:TokenValidityMins");
            var tokenExpiryTimeStamp=DateTime.UtcNow.AddMinutes(tokenValidityMins);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Name, request.UserName)
                }),
                Expires = tokenExpiryTimeStamp,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key!)), SecurityAlgorithms.HmacSha256Signature),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);

            return new LoginResponseModel
            {
                AccessToken = accessToken,
                UserName = request.UserName,
                ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.UtcNow).TotalSeconds,
            };
        }

    }
}
