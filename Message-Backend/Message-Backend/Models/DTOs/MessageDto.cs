using Message_Backend.Models.Enums;

namespace Message_Backend.Models.DTOs;

public class MessageDto
{
    public long MessageId { get; set; }
    public int SenderId { get; set; }
    public int ChatId { get; set; }
    public byte[] Content { get; set; }
    public MessageStatus Status { get; set; }
    public MessageType Type { get; set; }
}