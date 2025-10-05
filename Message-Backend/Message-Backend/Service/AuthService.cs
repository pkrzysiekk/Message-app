using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Message_Backend.Models;
using Microsoft.IdentityModel.Tokens;

namespace Message_Backend.Service;

public class AuthService: IAuthService
{
    public string GenerateToken(User user)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("dsfdsjdsfjsdfjsdfjsdfjsdjfsdjfsdjfjsdjfsdjfjsdfjsdfsdfsd");
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature
        );
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = GenerateClaims(user),
            Expires = DateTime.Now.AddHours(1),
            SigningCredentials = credentials
        };
        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }

    private ClaimsIdentity GenerateClaims(User user)
    {
        var claims = new ClaimsIdentity();
        claims.AddClaim(new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()));
        claims.AddClaim(new Claim(ClaimTypes.Email,user.Email));
        return claims;
    }
}