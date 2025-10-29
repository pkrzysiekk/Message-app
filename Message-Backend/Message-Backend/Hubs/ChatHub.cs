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
    private readonly IChatService _chatService;
   
    public ChatHub
        (IMessageService messageService, IChatService chatService)
    {
        _messageService = messageService;
        _chatService = chatService;
    }

    public override async Task OnConnectedAsync()
    {
        var callersId = Context.User.Identity.Name;
        if (callersId is null)
            return;
        
        var userId = Int32.Parse(callersId);
        var userChats = await _chatService.GetUserChats(userId);
        foreach (var userChat in userChats)
        {
             await Groups.AddToGroupAsync(Context.ConnectionId, userChat.Id.ToString());
        }
        await base.OnConnectedAsync();
    }
    
    public async Task SendMessageInChat(MessageDto message)
    {
        var messageBo = message.ToBo();
        await _messageService.Add(messageBo);
        
        await Clients.Group(message.ChatId.ToString()).ReceiveMessage(messageBo.ToDto());
    }
}