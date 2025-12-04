using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Message_Backend.Application.Helpers;
using Message_Backend.Application.Interfaces;
using Message_Backend.Application.Interfaces.Services;
using Message_Backend.Domain.Entities;
using Message_Backend.Domain.Exceptions;
using Message_Backend.Domain.Models.RSA;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Message_Backend.Application.Services;

public class AuthService: IAuthService
{
    private readonly UserManager<User> _userManager;
    IUserService _userService;

    public AuthService(UserManager<User> userManager, IUserService userService)
    {
       _userManager = userManager;
       _userService = userService;
    }
    public async Task<string> GenerateToken(JwtOptions jwtOptions,string username)
    {
        var handler = new JwtSecurityTokenHandler();
        var rsa=RsaHelper.PrivateKey;
        var user= await _userManager.FindByNameAsync(username);
        
        if(user is null)
            throw new NotFoundException("User not found");
        
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
        claims.AddClaim(new Claim(ClaimTypes.Name,user.UserName));
        return claims;
    }

    public async Task<bool> ValidateUserCredentials(string username, string password)
    {
        var userToCheck = await _userManager.FindByNameAsync(username);
        if (userToCheck == null)
            throw new NotFoundException("User not found");
        return await _userManager.CheckPasswordAsync(userToCheck,password);
    }

    public async Task RegisterUser(string username, string password)
    {
        var user= new User{UserName = username};
        await _userService.Add(user, password);
    }

}