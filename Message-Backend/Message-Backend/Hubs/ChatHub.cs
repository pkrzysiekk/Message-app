using Message_Backend.Hubs.Contracts;
using Message_Backend.Mappers;
using Message_Backend.Models.DTOs;
using Message_Backend.Service;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace Message_Backend.Hubs;
[SignalRHub]
public class ChatHub :Hub<IChatClient>
{
    private readonly IMessageService _messageService;
   
    public ChatHub(IMessageService messageService)
    {
        _messageService = messageService;
    }
    
    public async Task SendMessageInChat(MessageDto message)
    {
        var messageBo = message.ToBo();
        await _messageService.Add(messageBo);
        
        await Clients.Group(message.ChatId.ToString()).ReceiveMessage(messageBo.ToDto());
    }
}