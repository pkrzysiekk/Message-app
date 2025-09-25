namespace Message_Backend.Models.DTOs;

public class ChatDto
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public required string ChatName { get; set; }
}