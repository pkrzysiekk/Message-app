namespace Message_Backend.Models.DTOs;

public class UserAuthorizationRequest
{
    public required UserDto UserData { get; set; }
    public required string Password { get; set; }
}