using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Models;
using Microsoft.IdentityModel.Tokens;

namespace Services
{
    public class TokenManager : ITokenManager
    {
        #region snippet_ActionMethods

        public string GetJwt(User user)
        {
            var roles = string.Join(",", user.Roles.ToArray());
            var claims = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, roles),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            });
            string secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
                    SecurityAlgorithms.HmacSha256Signature
                ),
                Expires = DateTime.UtcNow.AddDays(2)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var createdToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(createdToken);
        }

        #endregion
    }
}
