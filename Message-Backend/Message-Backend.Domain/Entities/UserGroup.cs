using Message_Backend.Domain.Models.Enums;

namespace Message_Backend.Domain.Entities;

public class UserGroup
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int GroupId { get; set; }
    public GroupRole Role { get; set; }
    
    public User User { get; set; }
    public Group Group { get; set; }
}