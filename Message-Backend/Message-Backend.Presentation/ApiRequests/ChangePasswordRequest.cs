namespace Message_Backend.Presentation.ApiRequests;

public class ChangePasswordRequest
{
    public required string OldPassword { get; set; }
    public required string NewPassword { get; set; }
}