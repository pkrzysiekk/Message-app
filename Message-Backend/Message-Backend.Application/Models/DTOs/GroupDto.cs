using Message_Backend.Domain.Models.Enums;

namespace Message_Backend.Application.Models.DTOs;

public class GroupDto
{
    public int GroupId { get; set; }
    public required string GroupName { get; set; }
    public DateTime CreatedAt { get; set; }
    public GroupType GroupType { get; set; }
    public List<int> UsersInGroup { get; set; } = [];
}