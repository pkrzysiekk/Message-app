using System.Collections;

namespace Message_Backend.Models;

public class Group
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public Enum Type { get; set; }

    public ICollection<UserGroup> UserGroups { get; set; } = [];
    public ICollection<Chat> Chats { get; set; } = [];
}