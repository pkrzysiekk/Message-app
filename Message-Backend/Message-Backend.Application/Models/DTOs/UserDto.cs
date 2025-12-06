namespace Message_Backend.Application.Models.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public DateTime LastSeen { get; set; }
    public Byte[]? Avatar { get; set; }
    public bool IsOnline { get; set; }
}