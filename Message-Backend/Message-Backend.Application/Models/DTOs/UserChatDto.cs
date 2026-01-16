namespace Message_Backend.Application.Models.DTOs;

public class UserChatDto
{
    public int UserId { get; set; }
    public int ChatId { get; set; }
    public  int? LastMessageId { get; set; }
    public DateTime? LastReadAt { get; set; }  
}