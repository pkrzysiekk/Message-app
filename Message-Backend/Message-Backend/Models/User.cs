using System.Collections;

namespace Message_Backend.Models;

public class User
{
    public int Id { get; set; }
    public required string Username{ get; set; }
    public int AvatarId { get; set; }
    public DateTime LastSeen { get; set; }
    public bool IsOnline { get; set; }

    public ICollection<UserGroup> UserGroups { get; set; } = [];
    public ICollection<Message> Messages { get; set; } = [];
    public ICollection<Friends> Friends { get; set; } = [];
    public Avatar Avatar { get; set; }
}