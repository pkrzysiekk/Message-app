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
    private readonly IMessageAuthorizationService _messageAuthorizationService;
    private readonly IGroupService _groupService;
   
    public ChatHub
        (IMessageService messageService, IChatService chatService,
            IUserService userService, IMessageAuthorizationService authorizationService, IGroupService groupService)
    {
        _messageService = messageService;
        _chatService = chatService;
        _userService = userService;
        _messageAuthorizationService = authorizationService;
        _groupService = groupService;
    }

    public override async Task OnConnectedAsync()
    {
        await RefreshConnectionState(); 
        await base.OnConnectedAsync();
       // await _userService.ChangeOnlineStatus(userId, true); 
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var callersId = base.Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (callersId is null)
            return;
        await base.OnDisconnectedAsync(exception);
        var userId = Int32.Parse(callersId);
       // await _userService.ChangeOnlineStatus(userId, false);
    }

    public async Task RefreshConnectionState()
    {
        var callersId = base.Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (callersId is null)
            return;
        var userId = Int32.Parse(callersId);
        var userChats = await _chatService.GetUserChats(userId);
        var userGroups = await _groupService.GetUserGroups(userId);
        Context.Items["ChatIds"] = userChats.Select(x => x.Id);
        foreach (var userChat in userChats)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"chat:{userChat.Id}");
        }

        foreach (var userGroup in userGroups)
        {
           await Groups.AddToGroupAsync(Context.ConnectionId, $"group:{userGroup.Id}"); 
        }
    }

    public async Task JoinGroup(int groupId)
    {
        var userId = int.Parse(
            Context.User.FindFirstValue(ClaimTypes.NameIdentifier)
        );

        if (!await _messageAuthorizationService.IsUserInGroup(groupId, userId))
            throw new HubException("Unauthorized");

        await Groups.AddToGroupAsync(Context.ConnectionId, $"group:{groupId}");
    }

    public async Task JoinChat(int chatId)
    {
        var userId = int.Parse(
            Context.User.FindFirstValue(ClaimTypes.NameIdentifier)
        );
            if (!await _messageAuthorizationService.IsUserInChat(chatId, userId))
                throw new HubException("Unauthorized");
            await Groups.AddToGroupAsync(Context.ConnectionId, $"chat:{chatId}");
    }

    public async Task SendNewGroupRequest(int groupId)
    {
       await Clients.Group($"group:{groupId}").ReceiveAddToGroupEvent(groupId);
    }

    public async Task SendNewChatRequest(int chatId)
    {
        await Clients.Group($"chat:{chatId}").ReceiveAddToChatEvent(chatId);
    }
    

    public async Task SendMessage(MessageDto messageDto)
    {
        if (!isMessageRequestValid(Context, messageDto))
            throw new HubException("Invalid request");
        var callersId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        messageDto.SenderId = int.Parse(callersId);
        var messageBo = messageDto.ToBo();
        await _messageService.Add(messageBo);
        var finalMessage = messageBo.ToDto();
        finalMessage.SenderName = Context.User?.Identity?.Name;
        await Clients.Group($"chat:{messageBo.ChatId}").ReceiveMessage(finalMessage);
    }

    public async Task SendConnectionStateChanged(int groupId)
    {
        var callersId = base.Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (callersId is null)
            return;
        var userId = Int32.Parse(callersId);
        if (!await _messageAuthorizationService.IsUserInGroup(groupId, userId))
            return;
        await RefreshConnectionState();
        await Clients.Group($"group:{groupId}").ReceiveConnectionStateChanged();
    }

    public async Task SendUserIsTypingEvent(int chatId)
    {
        var username = base.Context.User?.Identity?.Name;
        if (username is null)
            return;
        await Clients.Group($"chat:{chatId.ToString()}").ReceiveUserIsTypingEvent(username);
    }

    public async Task RemoveMessage(long messageId)
    {
        var callersId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = int.Parse(callersId);
        if(!await _messageAuthorizationService.CanUserModifyMessage(messageId, userId))
           return;
        var message = await _messageService.GetById(messageId);
        await _messageService.Delete(messageId);
        await Clients.Group($"chat:{message.ChatId.ToString()}").ReceiveMessageRemovedEvent(message.ToDto());
        
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

        return userIsInChat;
    }

}