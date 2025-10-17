using Message_Backend.Models.Enums;

namespace Message_Backend.Models.DTOs;

public class UserGroupRoleRequest
{
    public required int GroupId { get; set; }
    public GroupRole Role { get; set; }
}