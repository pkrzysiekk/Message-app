using System.Security.Claims;
using Message_Backend.Application.Interfaces;
using Message_Backend.Application.Mappers;
using Message_Backend.Presentation.Hubs.Contracts;
using SignalRSwaggerGen.Attributes;
using System.Security.Claims;
using Message_Backend.Application.Interfaces.Services;
using Message_Backend.Application.Models.DTOs;
using Message_Backend.Application.Models.HubRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using static System.Security.Claims.ClaimsPrincipal;

namespace Message_Backend.Presentation.Hubs;
[SignalRHub]
[Authorize]
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
        var callersId = base.Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (callersId is null)
            return;
        var userId = Int32.Parse(callersId);
        var userChats = await _chatService.GetUserChats(userId);
        Context.Items["ChatIds"] = userChats.Select(x => x.Id);
        foreach (var userChat in userChats)
        {
             await Groups.AddToGroupAsync(Context.ConnectionId, userChat.Id.ToString());
        }
        
        await base.OnConnectedAsync();
       // await _userService.ChangeOnlineStatus(userId, true); 
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var callersId = base.Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
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
        if (!isMessageRequestValid(Context, messageDto))
            throw new HubException("Invalid request");
        var messageBo = messageDto.ToBo();
        await _messageService.Add(messageBo);
        SendMessageRequest request = new SendMessageRequest()
        {
            Message = messageBo.ToDto(),
            SenderName = base.Context.User.Identity.Name
        };
        
        await Clients.Group(messageBo.ChatId.ToString()).ReceiveMessage(request);
    }

    public async Task SendUserIsTypingEvent(int chatId)
    {
        var username = base.Context.User?.Identity?.Name;
        if (username is null)
            return;
        await Clients.Group(chatId.ToString()).ReceiveUserIsTypingEvent(username);
    }
    private bool isMessageRequestValid(HubCallerContext ctx, MessageDto messageDto)
    {
        var callersId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (callersId is null)
            return false;

        var userChatIdsObj = ctx.Items["ChatIds"];
        if (userChatIdsObj is null) 
            return false;

        var chatList = userChatIdsObj as IEnumerable<int>;
        if (chatList is null) 
            return false;

        bool userIsInChat = chatList.Contains(messageDto.ChatId);
        bool userIsCaller = messageDto.SenderId == int.Parse(callersId);

        return userIsInChat && userIsCaller;
    }

}