using Message_Backend.Models;
using Message_Backend.Models.RSA;

namespace Message_Backend.Service;

public interface IAuthService
{
    public Task<string> GenerateToken(JwtOptions jwtOptions,string username);
    public Task<bool> ValidateUserCredentials(string username, string password);
}