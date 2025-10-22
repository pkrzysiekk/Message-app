using System.Security.Claims;
using Message_Backend.AuthRequirements;
using Message_Backend.Helpers;
using Message_Backend.Models.DTOs;
using Message_Backend.Service;
using Microsoft.AspNetCore.Authorization;

namespace Message_Backend.AuthHandlers;

public class UserIsSenderHandler :AuthorizationHandler<UserIsSender>
{
    private readonly IMessageService _messageService;
    private readonly IChatService _chatService;
    private readonly IGroupService _groupService;
    public UserIsSenderHandler
        (IMessageService messageService, IChatService chatService, IGroupService groupService)
    {
        _messageService = messageService;
        _chatService = chatService;
        _groupService = groupService;
    }
    
    protected override async Task HandleRequirementAsync
        (AuthorizationHandlerContext context, AuthRequirements.UserIsSender requirement)
    {
        var callersId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (callersId is null ||
            context.Resource is not HttpContext httpContext )
        {
            context.Fail();
            return;
        }
        var messageId = Int32.Parse(httpContext.GetRouteValue("messageId").ToString());
        
        var fetchedMessage = await _messageService.GetById(messageId);
        var fetchedChat = await _chatService.Get(fetchedMessage.ChatId);
        var groupRole = await _groupService.GetUserRoleInGroup(Int32.Parse(callersId),fetchedMessage.ChatId);
        
        bool isSenderSameAsCaller = fetchedMessage.SenderId == Int32.Parse(callersId);
        bool hasRequiredRole = groupRole is not null && groupRole >= fetchedChat.ForRole;
        if (isSenderSameAsCaller && hasRequiredRole)
            context.Succeed(requirement);
        else
            context.Fail();
    }
}