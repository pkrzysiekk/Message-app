namespace Message_Backend.Models;

public class Message
{
    public int Id { get; set; }
    public int ChatId { get; set; }
    public int SenderId { get; set; }
    public required string Content { get; set; }
    public Enum Type { get; set; }
    public Enum Status { get; set; }
    public DateTime SentAt { get; set; }
    
    public Chat Chat { get; set; }
    public User Sender { get; set; }
    
    
}