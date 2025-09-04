using Ecommerce.Models.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ecommerce.Utilities
{
    public class GenerateJwtToken
    {
        private readonly IConfiguration _config;

        public GenerateJwtToken(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(User user, string tokenType)
        {
            var claims = new[]
            {
                   new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                   new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            DateTime expiry = tokenType == "Access"
                 ? DateTime.UtcNow.AddMinutes(15)
                 : DateTime.UtcNow.AddDays(7);


             var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
