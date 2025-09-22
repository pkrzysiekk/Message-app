using Microsoft.EntityFrameworkCore;

namespace Message_Backend.Models;
[Index(nameof(Name), IsUnique = true)]
public class Chat
{
    public int Id { get; set; }
    public int GroupId {get; set;}
    public required string Name { get; set; }
    
    public ICollection<Message> Messages { get; set; } = [];
    public Group Group {get; set;}
}