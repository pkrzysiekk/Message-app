using Message_Backend.Domain.Interfaces;

namespace Message_Backend.Domain.Entities;

public class UserChat :IEntity<int>
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ChatId { get; set; }
    public  int? LastMessageId { get; set; }
    public DateTime? LastReadAt { get; set; } 
    
    public Message? Message { get; set; }
    public Chat Chat { get; set; }
    public User User { get; set; }
}
