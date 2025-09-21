using Message_Backend.Models.Enums;

namespace Message_Backend.Models.DTOs;

public class GroupDto
{
    public int GroupId { get; set; }
    public required string GroupName { get; set; }
    public DateTime CreatedAt { get; set; }
    public GroupType GroupType { get; set; }
    public List<int> UsersInGroup { get; set; } = [];
}