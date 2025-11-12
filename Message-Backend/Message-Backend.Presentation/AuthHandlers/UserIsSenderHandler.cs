using System.Security.Claims;
using Message_Backend.Application.Interfaces;
using Message_Backend.Application.Interfaces.Services;
using Message_Backend.Domain.Entities;
using Message_Backend.Presentation.AuthRequirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Message_Backend.Presentation.AuthHandlers;

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
        var fetchedChat = await _chatService.GetById(fetchedMessage.ChatId);
        var groupRole = await _groupService.GetUserRoleInGroup(Int32.Parse(callersId),fetchedMessage.ChatId);
        
        bool isSenderSameAsCaller = fetchedMessage.SenderId == Int32.Parse(callersId);
        bool hasRequiredRole = groupRole is not null && groupRole >= fetchedChat.ForRole;
        if (isSenderSameAsCaller && hasRequiredRole)
            context.Succeed(requirement);
        else
            context.Fail();
    }
}