using System.Security.Claims;
using Message_Backend.Helpers;
using Message_Backend.Models;
using Message_Backend.Models.Enums;
using Message_Backend.Service;
using Microsoft.AspNetCore.Authorization;

namespace Message_Backend.AuthHandlers;

public class UserCanDeleteMessageHandler :AuthorizationHandler<UserCanDeleteMessage>
{
    private readonly IMessageService _messageService;
    private readonly IChatService _chatService;
    private readonly IGroupService _groupService;

    public UserCanDeleteMessageHandler
        (IMessageService messageService, IChatService chatService, IGroupService groupService)
    {
        _messageService = messageService;
        _chatService = chatService;
        _groupService = groupService;
    }
    
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserCanDeleteMessage requirement)
    {
        var callersId = context.User
            .FindFirstValue(ClaimTypes.NameIdentifier);
        var httpContext=RequestBodyHelper.GetHttpContext(context);
       
        if (callersId is null || httpContext is null)
        {
            context.Fail();
            return;
        } 
        bool parsed=
            Int32.TryParse(httpContext.GetRouteValue("messageId").ToString(),out int messageId);
        if (!parsed)
        {
            context.Fail();
            return;
        }
        var message = await _messageService.GetById(messageId);
        var chat = await _chatService.Get(message.ChatId);
        var group= await _groupService.GetById(chat.GroupId); 
        var userRole= await _groupService.GetUserRoleInGroup(int.Parse(callersId),group.Id);
        if (userRole is null)
        {
            context.Fail();
            return;
        }

        bool canDelete = Int32.Parse(callersId) == message.SenderId || userRole is GroupRole.Admin or GroupRole.Owner;
        if (canDelete)
            context.Succeed(requirement);
    }

    
}