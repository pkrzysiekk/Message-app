namespace Message_Backend.Models.DTOs;

public class AvatarDto
{
    public int UserId { get; set; }
    public required byte[] Content { get; set; }
}