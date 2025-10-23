namespace Message_Backend.Models.DTOs;

public class UserAuthorizationRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}