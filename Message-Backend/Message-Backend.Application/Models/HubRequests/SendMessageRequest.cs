using Message_Backend.Application.Models.DTOs;

namespace Message_Backend.Application.Models.HubRequests;

public class SendMessageRequest
{
    public MessageDto Message {get; set;}
    public string SenderName {get; set;}
}