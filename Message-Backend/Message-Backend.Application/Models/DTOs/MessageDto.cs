using Message_Backend.Domain.Models.Enums;

namespace Message_Backend.Application.Models.DTOs;

public class MessageDto
{
    public long MessageId { get; set; }
    public int SenderId { get; set; }
    public string? SenderName { get; set; }
    public int ChatId { get; set; }
    public byte[] Content { get; set; }
    public DateTime SentAt { get; set; }
    public MessageStatus Status { get; set; }
    public MessageType Type { get; set; }
}