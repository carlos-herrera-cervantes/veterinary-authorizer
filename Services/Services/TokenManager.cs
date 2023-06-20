using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Domain.Models;
using Domain.Constants;

namespace Services;

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
            new Claim(ClaimTypes.NameIdentifier, user.Id ?? user.Email)
        });

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            SigningCredentials = new SigningCredentials
            (
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtConfig.SecretKey)),
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
