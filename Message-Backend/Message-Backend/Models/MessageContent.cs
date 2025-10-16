namespace Message_Backend.Models;

public class MessageContent : IEntity<long>
{
    public long Id { get; set; }
    public long MessageId { get; set; }
    public byte[] Data { get; set; }
    
    public Message Message {get; set;}
}