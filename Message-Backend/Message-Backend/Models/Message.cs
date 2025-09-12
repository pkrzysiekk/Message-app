using Message_Backend.Models.Enums;

namespace Message_Backend.Models;

public class Message
{
    public int Id { get; set; }
    public int ChatId { get; set; }
    public int SenderId { get; set; }
    public required string Content { get; set; }
    public MessageType Type { get; set; }
    public MessageStatus Status { get; set; }
    public DateTime SentAt { get; set; }
    
    public Chat Chat { get; set; }
    public User Sender { get; set; }
    
    
}