using Message_Backend.Models.DTOs;

namespace Message_Backend.Models.HubRequests;

public class SendMessageRequest
{
    public MessageDto Message {get; set;}
    public string SenderName {get; set;}
}