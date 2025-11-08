namespace Message_Backend.Application.Models.DTOs;

public class AvatarDto
{
    public required byte[] Content { get; set; }
    public required string ContentType { get; set; }
}