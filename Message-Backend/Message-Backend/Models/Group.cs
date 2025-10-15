using System.Collections;
using Message_Backend.Models.Enums;

namespace Message_Backend.Models;

public class Group : IEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public GroupType Type { get; set; }
     
    public ICollection<UserGroup> UserGroups { get; set; } = [];
    public ICollection<Chat> Chats { get; set; } = [];
}