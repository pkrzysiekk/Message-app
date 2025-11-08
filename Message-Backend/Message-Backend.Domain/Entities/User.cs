using Microsoft.AspNetCore.Identity;

namespace Message_Backend.Domain.Entities;

public class User : IdentityUser<int>
{

    public int AvatarId { get; set; }
    public DateTime LastSeen { get; set; }
    public bool IsOnline { get; set; }=false;
    public ICollection<UserGroup> UserGroups { get; set; } = [];
    public ICollection<Message> Messages { get; set; } = [];
    public ICollection<Friends> Friends { get; set; } = [];
    public Avatar? Avatar { get; set; }

}