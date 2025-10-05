using Message_Backend.Models;

namespace Message_Backend.Service;

public interface IAuthService
{
    public string GenerateToken(User user);
}