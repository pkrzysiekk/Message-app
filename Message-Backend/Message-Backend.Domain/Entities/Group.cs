using Message_Backend.Domain.Interfaces;
using Message_Backend.Domain.Models.Enums;

namespace Message_Backend.Domain.Entities;

public class Group : IEntity<int>
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public GroupType Type { get; set; }
     
    public ICollection<UserGroup> UserGroups { get; set; } = [];
    public ICollection<Chat> Chats { get; set; } = [];
}