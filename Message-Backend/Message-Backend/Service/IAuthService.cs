using Message_Backend.Models;
using Message_Backend.Models.RSA;

namespace Message_Backend.Service;

public interface IAuthService
{
    public string GenerateToken(JwtOptions jwtOptions,User user);
    public Task<bool> ValidateUserCredentials(User user, string password);
}