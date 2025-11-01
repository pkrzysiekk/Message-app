using System.Security.Claims;
using Message_Backend.Hubs.Contracts;
using Message_Backend.Mappers;
using Message_Backend.Models.DTOs;
using Message_Backend.Models.HubRequests;
using Message_Backend.Service;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace Message_Backend.Hubs;
[SignalRHub]
public class ChatHub :Hub<IChatClient>
{
    private readonly IMessageService _messageService;
    private readonly IChatService _chatService;
    private readonly IUserService _userService;
   
    public ChatHub
        (IMessageService messageService, IChatService chatService, IUserService userService)
    {
        _messageService = messageService;
        _chatService = chatService;
        _userService = userService;
    }

    public override async Task OnConnectedAsync()
    {
        var callersId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (callersId is null)
            return;
        
        var userId = Int32.Parse(callersId);
        var userChats = await _chatService.GetUserChats(userId);
        foreach (var userChat in userChats)
        {
             await Groups.AddToGroupAsync(Context.ConnectionId, userChat.Id.ToString());
        }
        
        await base.OnConnectedAsync();
       // await _userService.ChangeOnlineStatus(userId, true); 
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var callersId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (callersId is null)
            return;
        var userChats = await _chatService.GetUserChats(Int32.Parse(callersId));
        foreach (var userChat in userChats)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userChat.Id.ToString());
        }
        await base.OnDisconnectedAsync(exception);
        var userId = Int32.Parse(callersId);
       // await _userService.ChangeOnlineStatus(userId, false);
    }

    public async Task SendMessage(MessageDto messageDto)
    {
        var messageBo = messageDto.ToBo();
        await _messageService.Add(messageBo);
        SendMessageRequest request = new SendMessageRequest()
        {
            Message = messageBo.ToDto(),
            SenderName = Context.User.Identity.Name
        };
        
        await Clients.Group(messageBo.ChatId.ToString()).ReceiveMessage(request);
    }
}