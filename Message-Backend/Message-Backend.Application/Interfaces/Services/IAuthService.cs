using Message_Backend.Domain.Models.RSA;

namespace Message_Backend.Application.Interfaces.Services;

public interface IAuthService
{
    public Task<string> GenerateToken(JwtOptions jwtOptions,string username);
    public Task<bool> ValidateUserCredentials(string username, string password);
}