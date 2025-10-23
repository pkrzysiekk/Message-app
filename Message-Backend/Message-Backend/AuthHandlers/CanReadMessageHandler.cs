using System.Security.Claims;
using Message_Backend.AuthRequirements;
using Message_Backend.Service;
using Microsoft.AspNetCore.Authorization;

namespace Message_Backend.AuthHandlers;

public class CanReadMessageHandler :AuthorizationHandler<CanReadMessage>
{
    private readonly IChatService  _chatService;
    private readonly IGroupService _groupService;
    private readonly IMessageService _messageService;

    public CanReadMessageHandler
        (IChatService chatService, IGroupService groupService, IMessageService messageService)
    {
        _chatService = chatService;
        _groupService = groupService;
        _messageService = messageService;
    }
    
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CanReadMessage requirement)
    {
        var callersId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (callersId is null ||
            context.Resource is not HttpContext httpContext )
        {
            context.Fail();
            return;
        }

        int userId=int.Parse(callersId); 
        var messageId = Int32.Parse(httpContext.GetRouteValue("messageId").ToString());
        var message= await _messageService.GetById(messageId);
        var chat = await _chatService.Get(message.ChatId);
        var userRole = await _groupService.GetUserRoleInGroup(userId,chat.GroupId);

        if (userRole >= chat.ForRole)
            context.Succeed(requirement);
    }
}