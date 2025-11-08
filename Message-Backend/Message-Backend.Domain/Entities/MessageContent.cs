using Message_Backend.Domain.Interfaces;

namespace Message_Backend.Domain.Entities;

public class MessageContent : IEntity<long>
{
    public long Id { get; set; }
    public byte[] Data { get; set; }
    
    public Message Message {get; set;}
}