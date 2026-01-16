using Message_Backend.Domain.Interfaces;
using Message_Backend.Domain.Models.Enums;

namespace Message_Backend.Domain.Entities;
public class Chat :IEntity<int>
{
    public int Id { get; set; }
    public int GroupId {get; set;}
    public required string Name { get; set; }
    public GroupRole ForRole { get; set; }
    
    public ICollection<Message> Messages { get; set; } = [];
    public ICollection<UserChat> UserChats { get; set; } = [];
    public Group Group {get; set;}
}