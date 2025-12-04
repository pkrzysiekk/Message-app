namespace Message_Backend.Presentation.ApiRequests;

public class RegisterRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
}