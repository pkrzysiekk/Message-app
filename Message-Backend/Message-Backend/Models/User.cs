using System.Collections;
using Microsoft.AspNetCore.Identity;

namespace Message_Backend.Models;

public class User : IdentityUser<int>
{

    public int AvatarId { get; set; }
    public DateTime LastSeen { get; set; }
    public bool IsOnline { get; set; }
    public ICollection<UserGroup> UserGroups { get; set; } = [];
    public ICollection<Message> Messages { get; set; } = [];
    public ICollection<Friends> Friends { get; set; } = [];
    public Avatar? Avatar { get; set; }

}