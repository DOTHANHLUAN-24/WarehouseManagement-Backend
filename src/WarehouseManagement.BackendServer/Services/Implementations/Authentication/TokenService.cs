using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WarehouseManagement.BackendServer.Repositories.Implements.Authentication;
using WarehouseManagement.BackendServer.Services.Interfaces.Authentication;

namespace WarehouseManagement.BackendServer.Services.Implementations.Authentication
{
    public class TokenService(IConfiguration configuration, IRefreshTokenRepository refreshTokenRepository) : ITokenService
    {
        public string GenerateAccessToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["JWT:Key"]!)
            );

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["JWT:Issuer"],
                audience: configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: cred
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);

            return WebUtility.UrlEncode(Convert.ToBase64String(bytes));
        }

        public async Task<bool> ValidateRefreshToken(string token)
        {
            return await refreshTokenRepository.Validate(token);
        }

        public async Task<string?> GetUserIdFromRefreshToken(string token)
        {
            return await refreshTokenRepository.GetUserIdByToken(token);
        }
    }
}
