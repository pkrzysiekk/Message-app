using Message_Backend.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Message_Backend.Models;
public class Chat :IEntity
{
    public int Id { get; set; }
    public int GroupId {get; set;}
    public required string Name { get; set; }
    public GroupRole ForRole { get; set; }
    
    public ICollection<Message> Messages { get; set; } = [];
    public Group Group {get; set;}
}