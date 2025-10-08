using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Message_Backend.Exceptions;
using Message_Backend.Helpers;
using Message_Backend.Models;
using Message_Backend.Models.RSA;
using Message_Backend.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Message_Backend.Service;

public class AuthService: IAuthService
{
    private readonly UserManager<User> _userManager;

    public AuthService(UserManager<User> userManager, IUserService userService)
    {
       _userManager = userManager;
    }
    public string GenerateToken(JwtOptions jwtOptions,User user)
    {
        var handler = new JwtSecurityTokenHandler();
        var rsa=RsaHelper.LoadRsaKey(jwtOptions.PrivateKeyLocation);
        
        var credentials = new SigningCredentials(
            new RsaSecurityKey(rsa),
            SecurityAlgorithms.RsaSha256
        );
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = GenerateClaims(user),
            Expires = DateTime.Now.AddMinutes(jwtOptions.MinutesBeforeExpiry),
            Issuer = jwtOptions.Issuer,
            Audience = jwtOptions.Audience,
            SigningCredentials = credentials
        };
        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }

    private static ClaimsIdentity GenerateClaims(User user)
    {
        var claims = new ClaimsIdentity();
        claims.AddClaim(new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()));
        claims.AddClaim(new Claim(ClaimTypes.Email,user.Email));
        return claims;
    }

    public async Task<bool> ValidateUserCredentials(User user, string password)
    {
        var userToCheck = await _userManager.FindByIdAsync(user.Id.ToString());
        if (userToCheck == null)
            throw new NotFoundException("User not found");
        return await _userManager.CheckPasswordAsync(userToCheck,password);
    }

}