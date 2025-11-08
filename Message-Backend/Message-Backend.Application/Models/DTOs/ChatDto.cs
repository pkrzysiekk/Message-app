using Message_Backend.Domain.Models.Enums;

namespace Message_Backend.Application.Models.DTOs;

public class ChatDto
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public required string ChatName { get; set; }
    public GroupRole ForRole { get; set; }
}