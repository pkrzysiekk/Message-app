using Message_Backend.Domain.Models.Enums;

namespace Message_Backend.Presentation.ApiRequests;

public class UserGroupRoleRequest
{
    public required int GroupId { get; set; }
    public GroupRole GroupRole { get; set; }
}