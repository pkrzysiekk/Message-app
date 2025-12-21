using Message_Backend.Application.Models.DTOs;

namespace Message_Backend.Presentation.ApiRequests;

public class UserGroupRequest
{
    public GroupDto Group { get; set; }
    public int userId { get; set; }
}