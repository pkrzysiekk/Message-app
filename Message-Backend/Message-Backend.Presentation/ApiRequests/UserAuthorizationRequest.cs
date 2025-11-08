namespace Message_Backend.Presentation.ApiRequests;

public class UserAuthorizationRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}